using Claunia.PropertyList;
using System.Collections;
using System.Reflection;

namespace TTR.NSKeyedUnarchiver
{
    /// <summary>
    /// Hacky NSKeyedArchiver implementation written with multiple AI models
    /// Then fixed up by Arktisfox
    /// It works for what it was built for, that's what matters
    /// </summary>
    public static class Archiver
    {
        private static List<NSObject> _objects;
        private static Dictionary<int, int> _uidCache; // hashCode -> object index
        private static Dictionary<string, int> _classCache;

        // Encodes any C# object into an NSKeyedArchiver-compatible NSDictionary.
        public static NSDictionary Archive(object root)
        {
            _objects = new List<NSObject>();
            _uidCache = new Dictionary<int, int>();
            _classCache = new Dictionary<string, int>();

            // $null is always object 0
            _objects.Add(new NSString("$null"));

            var topUid = EncodeObject(root);
            var topDict = new NSDictionary
            {
                { "root", topUid }
            };

            var objectsArray = new NSArray(_objects.Count);
            for (int i = 0; i < _objects.Count; i++)
            {
                objectsArray.Add(_objects[i]);
            }

            var archiverDict = new NSDictionary
            {
                { "$archiver", new NSString("NSKeyedArchiver") },
                { "$version", new NSNumber(100000) },
                { "$objects", objectsArray },
                { "$top", topDict }
            };

            return archiverDict;
        }

        // Recursively encodes an object and returns a UID pointing to it in $objects.
        private static UID EncodeObject(object value)
        {
            if (value == null)
                return MakeUID(0);

            // Inline primitives directly
            var inlined = ConvertToNSInline(value);
            if (inlined != null)
            {
                _objects.Add(inlined);
                return MakeUID(_objects.Count - 1);
            }

            int hash = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(value);

            if (_uidCache.TryGetValue(hash, out int existing))
                return MakeUID(existing);

            // Reserve FIRST
            int index = _objects.Count;
            _objects.Add(null);
            _uidCache[hash] = index;

            // THEN encode
            NSObject encoded = ConvertToNS(value);

            _objects[index] = encoded;

            return MakeUID(index);
        }

        // Converts a C# object to its closest NSObject equivalent.
        private static NSObject ConvertToNS(object value)
        {
            switch (value)
            {
                case bool b:
                    return new NSNumber(b);
                case long l:
                    return new NSNumber(l);
                case int i:
                    return new NSNumber((long)i);
                case double d:
                    return new NSNumber(d);
                case float f:
                    return new NSNumber((double)f);
                case string s:
                    return new NSString(s);
                case byte[] bytes:
                    return new NSData(bytes);
                case DateTime dt:
                    return new NSDate(dt);
                case Array arr:
                    return EncodeArray(arr.Cast<object>().ToArray());
                case IList list:
                    return EncodeArray(list.Cast<object>().ToArray());
                case IDictionary dict:
                    return EncodeDictionary(dict);
                case ISet<object> set:
                    return EncodeSet(set, sorted: set is SortedSet<object>);
                default:
                    return EncodeArbitraryObject(value);
            }
        }

        private static NSObject? ConvertToNSInline(object value)
        {
            // These types should be inlined directly, not stored as UID references
            return value switch
            {
                null => new UID(0),          // $null reference
                bool b => new NSNumber(b),
                long l => new NSNumber(l),
                int i => new NSNumber((long)i),
                double d => new NSNumber(d),
                float f => new NSNumber((double)f),
                string s => new NSString(s),
                byte[] bytes => new NSData(bytes),
                DateTime dt => new NSDate(dt),
                _ => null                    // not a primitive, fall through to EncodeObject
            };
        }

        private static NSObject EncodeValue(object value)
        {
            return ConvertToNSInline(value) ?? EncodeObject(value);
        }

        // Encodes a list/array as an NSMutableArray wrapper dict.
        private static NSDictionary EncodeArray(object[] items)
        {
            var nsObjects = new NSArray(items.Length);
            for (int i = 0; i < items.Length; i++)
                nsObjects.Add(EncodeObject(items[i]));

            var dict = new NSDictionary
            {
                { "$class", ClassRef("NSMutableArray") },
                { "NS.objects", nsObjects }
            };
            return dict;
        }

        // Encodes a dictionary as an NSMutableDictionary wrapper dict.
        private static NSDictionary EncodeDictionary(IDictionary source)
        {
            var keys = new NSArray(source.Count);
            var objects = new NSArray(source.Count);
            foreach (DictionaryEntry entry in source)
            {
                keys.Add(EncodeObject(entry.Key));
                objects.Add(EncodeValue(entry.Value));
            }

            var dict = new NSDictionary();
            dict.Add("$class", ClassRef("NSMutableDictionary"));
            dict.Add("NS.keys", keys);
            dict.Add("NS.objects", objects);
            return dict;
        }

        // Encodes a set as an NSSet or NSOrderedSet wrapper dict.
        private static NSDictionary EncodeSet(IEnumerable<object> source, bool sorted)
        {
            var items = source.ToArray();
            var nsObjects = new NSArray(items.Length);
            for (int i = 0; i < items.Length; i++)
                nsObjects.Add(EncodeObject(items[i]));

            var dict = new NSDictionary();
            dict.Add("$class", ClassRef(sorted ? "NSOrderedSet" : "NSSet"));
            dict.Add("NS.objects", nsObjects);
            return dict;
        }

        private static NSDictionary EncodeArbitraryObject(object obj)
        {
            var type = obj.GetType();
            var dict = new NSDictionary();
            dict.Add("$class", ClassRef(type.Name));

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;
                var propValue = prop.GetValue(obj);
                dict.Add(prop.Name, EncodeValue(propValue));
            }

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var fieldValue = field.GetValue(obj);
                dict.Add(field.Name, EncodeValue(fieldValue));
            }

            return dict;
        }

        // Builds a $class reference dict and returns it as a UID.
        private static UID ClassRef(string className)
        {
            if (_classCache.TryGetValue(className, out int existing))
                return MakeUID(existing);

            var classDict = new NSDictionary
            {
                { "$classname", new NSString(className) },
                { "$classes", new NSArray(new NSObject[]
                    {
                        new NSString(className),
                        new NSString("NSObject")
                    })
                }
            };

            int index = _objects.Count;
            _objects.Add(classDict);

            _classCache[className] = index;

            return MakeUID(index);
        }

        private static UID MakeUID(int index)
        {
            if(index <= byte.MaxValue)
            {
                return new UID((byte)index);
            }
            else
            {
                return new UID((uint)index);
            }
        }
    }
}