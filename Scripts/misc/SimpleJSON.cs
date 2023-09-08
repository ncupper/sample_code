#if !UNITY_WEBPLAYER && !UNITY_WEBGL
#define USE_FileIO
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using UnityEngine;
namespace misc
{
    public enum JSONBinaryTag
    {
        Array = 1,
        Class = 2,
        Value = 3,
        IntValue = 4,
        DoubleValue = 5,
        BoolValue = 6,
        FloatValue = 7
    }

    public class JSONNode
    {
        #region common interface

        public virtual void Add(string aKey, JSONNode aItem) {}

        public void Add(string aKey, int aItem) { Add(aKey, aItem.ToString()); }

        public void Add(string aKey, long aItem) { Add(aKey, aItem.ToString()); }

        public void Add(string aKey, float aItem) { Add(aKey, aItem.ToString()); }

        public void Add(string aKey, double aItem) { Add(aKey, aItem.ToString()); }

        public void Add(string aKey, bool aItem) { Add(aKey, aItem.ToString()); }

        public void Add<T>(string aKey, T[] arItem)
        {
            var ar = new JSONArray();
            if (arItem != null)
            {
                foreach (T it in arItem)
                {
                    ar.Add(it != null ? it.ToString() : "");
                }
            }
            Add(aKey, ar);
        }

        public void Add(string aKey, List<string> arItem)
        {
            var ar = new JSONArray();
            foreach (string it in arItem)
            {
                ar.Add(it);
            }
            Add(aKey, ar);
        }

        public void Add(string aKey, List<int> arItem)
        {
            var ar = new JSONArray();
            if (arItem != null)
            {
                foreach (int it in arItem)
                {
                    ar.Add(it.ToString());
                }
            }
            Add(aKey, ar);
        }

        public void Add<T>(string aKey, IReadOnlyList<T> arItem, Func<T, JSONNode> saver)
        {
            var ar = new JSONArray();
            foreach (T it in arItem)
            {
                if (it != null)
                {
                    ar.Add(saver(it));
                }
            }
            Add(aKey, ar);
        }

        public void Add(string aKey, Vector3 aItem)
        {
            float[] coords =
            {
                aItem.x, aItem.y,
                aItem.z
            };
            Add(aKey, coords);
        }
        public virtual JSONNode this[int aIndex]
        {
            get => null;
            set {}
        }
        public virtual JSONNode this[string aKey]
        {
            get => null;
            set {}
        }
        public virtual string Value
        {
            get => "";
            set {}
        }
        public virtual int Count => 0;

        public virtual void Add(JSONNode aItem)
        {
            Add("", aItem);
        }

        public virtual JSONNode Remove(string aKey) { return null; }

        public virtual JSONNode Remove(int aIndex) { return null; }

        public virtual JSONNode Remove(JSONNode aNode) { return aNode; }

        public virtual IEnumerable<JSONNode> Childs { get { yield break; } }
        public virtual IEnumerable<KeyValuePair<string, JSONNode>> PairChilds { get { yield break; } }
        public IEnumerable<JSONNode> DeepChilds
        {
            get
            {
                foreach (JSONNode C in Childs)
                foreach (JSONNode D in C.DeepChilds)
                {
                    yield return D;
                }
            }
        }

        public override string ToString()
        {
            return "JSONNode";
        }
        public virtual string ToString(string aPrefix)
        {
            return "JSONNode";
        }

        #endregion common interface

        #region typecasting properties

        public virtual int AsInt
        {
            get
            {
                var v = 0;
                if (int.TryParse(Value, out v))
                {
                    return v;
                }
                return 0;
            }
            set => Value = value.ToString();
        }

        public virtual long AsLong
        {
            get
            {
                long v = 0;
                if (long.TryParse(Value, out v))
                {
                    return v;
                }
                return 0;
            }
            set => Value = value.ToString();
        }
        public virtual float AsFloat
        {
            get
            {
                var v = 0.0f;
                if (float.TryParse(Value, NumberStyles.Float, new CultureInfo("en-US"), out v))
                {
                    return v;
                }
                return 0.0f;
            }
            set => Value = value.ToString(new CultureInfo("en-US"));
        }
        public virtual double AsDouble
        {
            get
            {
                var v = 0.0;
                if (double.TryParse(Value, out v))
                {
                    return v;
                }
                return 0.0;
            }
            set => Value = value.ToString();
        }
        public virtual bool AsBool
        {
            get
            {
                var v = false;
                if (bool.TryParse(Value, out v))
                {
                    return v;
                }
                return !string.IsNullOrEmpty(Value);
            }
            set => Value = value ? "true" : "false";
        }
        public virtual JSONArray AsArray => this as JSONArray;
        public virtual JSONClass AsObject => this as JSONClass;

        #endregion typecasting properties

        #region operators

        public static implicit operator JSONNode(string s)
        {
            return new JSONData(s);
        }
        public static implicit operator string(JSONNode d)
        {
            return d == null ? null : d.Value;
        }
        public static bool operator ==(JSONNode a, object b)
        {
            if (b == null && a is JSONLazyCreator)
            {
                return true;
            }
            return ReferenceEquals(a, b);
        }

        public static bool operator !=(JSONNode a, object b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion operators

        public string GetKey(string name, string def = "", bool slashN = false)
        {
            if (this[name] == null)
            {
                return def;
            }
            if (slashN)
            {
                var len = 0;
                do
                {
                    len = this[name].Value.Length;
                    this[name].Value = this[name].Value.Replace("\\n", "\n");
                }
                while (len > this[name].Value.Length);
            }
            return this[name].Value;
        }

        public T GetEnum<T>(string name, T def)
        {
            if (this[name] == null)
            {
                return def;
            }
            try
            {
                return (T)Enum.Parse(typeof(T), GetKey(name), true);
            }
            catch (ArgumentException)
            {
                return def;
            }
        }

        public int GetInt(string name, int def = 0)
        {
            if (this[name] == null)
            {
                return def;
            }
            return this[name].AsInt;
        }

        public float GetFloat(string name, float def = 0)
        {
            if (this[name] == null)
            {
                return def;
            }
            return this[name].AsFloat;
        }

        public double GetDouble(string name, double def = 0)
        {
            if (this[name] == null)
            {
                return def;
            }
            return this[name].AsDouble;
        }

        public long GetLong(string name, long def = 0)
        {
            if (this[name] == null)
            {
                return def;
            }
            return this[name].AsLong;
        }

        public bool GetBool(string name, bool def = false)
        {
            if (this[name] == null)
            {
                return def;
            }
            return this[name].AsBool;
        }

        public int[] GetIntArray(string name, int def, int minSize)
        {
            JSONArray ar = GetArray(name);
            int size = Math.Max(minSize, ar.Count);
            var result = new int[size];
            for (var i = 0; i < size; ++i)
            {
                result[i] = i < ar.Count ? ar[i].AsInt : def;
            }
            return result;
        }

        public List<int> GetIntList(string name, int def, int minSize)
        {
            JSONArray ar = GetArray(name);
            int size = Math.Max(minSize, ar.Count);
            var result = new List<int>();
            for (var i = 0; i < size; ++i)
            {
                result.Add(i < ar.Count ? ar[i].AsInt : def);
            }
            return result;
        }

        public List<string> GetStrArray(string name, string def, int minSize)
        {
            JSONArray ar = GetArray(name);
            int size = Math.Max(minSize, ar.Count);
            var result = new List<string>();
            for (var i = 0; i < size; ++i)
            {
                result.Add(i < ar.Count ? ar[i].Value : def);
            }
            return result;
        }

        public JSONArray GetArray(string name)
        {
            if (this[name] == null || this[name].AsArray == null)
            {
                return new JSONArray();
            }
            return this[name].AsArray;
        }

        public Vector3 GetVector(string name, Vector3 def)
        {
            JSONArray ar = GetArray(name);
            if (ar.Count == 2)
            {
                return new Vector3(ar[0].AsFloat, ar[1].AsFloat, 0);
            }
            if (ar.Count != 3)
            {
                return def;
            }
            return new Vector3(ar[0].AsFloat, ar[1].AsFloat, ar[2].AsFloat);
        }

        public Dictionary<int, T> GetDict<T>(string name, string key, Func<JSONNode, T> initer)
        {
            JSONNode r = this[name];
            var result = new Dictionary<int, T>();
            foreach (JSONNode d in r.Childs)
            {
                result.Add(d.GetInt(key), initer(d));
            }
            return result;
        }

        public List<T> GetList<T>(string name, Func<JSONNode, T> initer)
        {
            var result = new List<T>();
            JSONArray ar = this[name].AsArray;
            for (var i = 0; i < ar.Count; ++i)
            {
                result.Add(initer(ar[i]));
            }
            return result;
        }

        internal static string Escape(string aText)
        {
            var result = "";
            foreach (char c in aText)
            {
                switch (c)
                {
                case '\\':
                    result += "\\\\";
                    break;
                case '\"':
                    result += "\\\"";
                    break;
                case '\n':
                    result += "\\n";
                    break;
                case '\r':
                    result += "\\r";
                    break;
                case '\t':
                    result += "\\t";
                    break;
                case '\b':
                    result += "\\b";
                    break;
                case '\f':
                    result += "\\f";
                    break;
                default:
                    result += c;
                    break;
                }
            }
            return result;
        }

        public static JSONNode Parse(string aJSON)
        {
            var stack = new Stack<JSONNode>();
            JSONNode ctx = null;
            var i = 0;
            var Token = "";
            var TokenName = "";
            var QuoteMode = false;
            while (i < aJSON.Length)
            {
                switch (aJSON[i])
                {
                case '{':
                    if (QuoteMode)
                    {
                        Token += aJSON[i];
                        break;
                    }
                    stack.Push(new JSONClass());
                    if (ctx != null)
                    {
                        TokenName = TokenName.Trim();
                        if (ctx is JSONArray)
                        {
                            ctx.Add(stack.Peek());
                        }
                        else if (TokenName != "")
                        {
                            ctx.Add(TokenName, stack.Peek());
                        }
                    }
                    TokenName = "";
                    Token = "";
                    ctx = stack.Peek();
                    break;

                case '[':
                    if (QuoteMode)
                    {
                        Token += aJSON[i];
                        break;
                    }

                    stack.Push(new JSONArray());
                    if (ctx != null)
                    {
                        TokenName = TokenName.Trim();
                        if (ctx is JSONArray)
                        {
                            ctx.Add(stack.Peek());
                        }
                        else if (TokenName != "")
                        {
                            ctx.Add(TokenName, stack.Peek());
                        }
                    }
                    TokenName = "";
                    Token = "";
                    ctx = stack.Peek();
                    break;

                case '}':
                case ']':
                    if (QuoteMode)
                    {
                        Token += aJSON[i];
                        break;
                    }
                    if (stack.Count == 0)
                    {
                        throw new Exception("JSON Parse: Too many closing brackets");
                    }

                    stack.Pop();
                    if (Token != "")
                    {
                        TokenName = TokenName.Trim();
                        if (ctx is JSONArray)
                        {
                            ctx.Add(Token);
                        }
                        else if (TokenName != "")
                        {
                            ctx.Add(TokenName, Token);
                        }
                    }
                    TokenName = "";
                    Token = "";
                    if (stack.Count > 0)
                    {
                        ctx = stack.Peek();
                    }
                    break;

                case ':':
                    if (QuoteMode)
                    {
                        Token += aJSON[i];
                        break;
                    }
                    TokenName = Token;
                    Token = "";
                    break;

                case '"':
                    QuoteMode ^= true;
                    break;

                case ',':
                    if (QuoteMode)
                    {
                        Token += aJSON[i];
                        break;
                    }
                    if (Token != "")
                    {
                        if (ctx is JSONArray)
                        {
                            ctx.Add(Token);
                        }
                        else if (TokenName != "")
                        {
                            ctx.Add(TokenName, Token);
                        }
                    }
                    TokenName = "";
                    Token = "";
                    break;

                case '\r':
                case '\n':
                    break;

                case ' ':
                case '\t':
                    if (QuoteMode)
                    {
                        Token += aJSON[i];
                    }
                    break;

                case '\\':
                    ++i;
                    if (QuoteMode)
                    {
                        char C = aJSON[i];
                        switch (C)
                        {
                        case 't':
                            Token += '\t';
                            break;
                        case 'r':
                            Token += '\r';
                            break;
                        case 'n':
                            Token += '\n';
                            break;
                        case 'b':
                            Token += '\b';
                            break;
                        case 'f':
                            Token += '\f';
                            break;
                        case 'u':
                        {
                            string s = aJSON.Substring(i + 1, 4);
                            Token += (char)int.Parse(s, NumberStyles.AllowHexSpecifier);
                            i += 4;
                            break;
                        }
                        default:
                            Token += C;
                            break;
                        }
                    }
                    break;

                default:
                    Token += aJSON[i];
                    break;
                }
                ++i;
            }
            if (QuoteMode)
            {
                throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            }
            return ctx;
        }

        public virtual void Serialize(BinaryWriter aWriter) {}

        public void SaveToStream(Stream aData)
        {
            var W = new BinaryWriter(aData);
            Serialize(W);
        }

#if USE_SharpZipLib
        public void SaveToCompressedStream(System.IO.Stream aData)
        {
            using (var gzipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(aData))
            {
                gzipOut.IsStreamOwner = false;
                SaveToStream(gzipOut);
                gzipOut.Close();
            }
        }

        public void SaveToCompressedFile(string aFileName)
        {
#if USE_FileIO
            System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
            using(var F = System.IO.File.OpenWrite(aFileName))
            {
                SaveToCompressedStream(F);
            }
#else
            throw new Exception("Can't use File IO stuff in webplayer");
#endif
        }
        public string SaveToCompressedBase64()
        {
            using (var stream = new System.IO.MemoryStream())
            {
                SaveToCompressedStream(stream);
                stream.Position = 0;
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }

#else
        public void SaveToCompressedStream(Stream aData)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
        public void SaveToCompressedFile(string aFileName)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
        public string SaveToCompressedBase64()
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
#endif

        public void SaveToFile(string aFileName)
        {
#if USE_FileIO
            Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
            using (FileStream F = File.OpenWrite(aFileName))
            {
                SaveToStream(F);
            }
#else
            throw new Exception("Can't use File IO stuff in webplayer");
#endif
        }
        public string SaveToBase64()
        {
            using (var stream = new MemoryStream())
            {
                SaveToStream(stream);
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }
        public static JSONNode Deserialize(BinaryReader aReader)
        {
            var type = (JSONBinaryTag)aReader.ReadByte();
            switch (type)
            {
            case JSONBinaryTag.Array:
            {
                int count = aReader.ReadInt32();
                var tmp = new JSONArray();
                for (var i = 0; i < count; i++)
                {
                    tmp.Add(Deserialize(aReader));
                }
                return tmp;
            }
            case JSONBinaryTag.Class:
            {
                int count = aReader.ReadInt32();
                var tmp = new JSONClass();
                for (var i = 0; i < count; i++)
                {
                    string key = aReader.ReadString();
                    JSONNode val = Deserialize(aReader);
                    tmp.Add(key, val);
                }
                return tmp;
            }
            case JSONBinaryTag.Value:
            {
                return new JSONData(aReader.ReadString());
            }
            case JSONBinaryTag.IntValue:
            {
                return new JSONData(aReader.ReadInt32());
            }
            case JSONBinaryTag.DoubleValue:
            {
                return new JSONData(aReader.ReadDouble());
            }
            case JSONBinaryTag.BoolValue:
            {
                return new JSONData(aReader.ReadBoolean());
            }
            case JSONBinaryTag.FloatValue:
            {
                return new JSONData(aReader.ReadSingle());
            }

            default:
            {
                throw new Exception("Error deserializing JSON. Unknown tag: " + type);
            }
            }
        }

#if USE_SharpZipLib
        public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
        {
            var zin = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(aData);
            return LoadFromStream(zin);
        }
        public static JSONNode LoadFromCompressedFile(string aFileName)
        {
#if USE_FileIO
            using(var F = System.IO.File.OpenRead(aFileName))
            {
                return LoadFromCompressedStream(F);
            }
#else
            throw new Exception("Can't use File IO stuff in webplayer");
#endif
        }
        public static JSONNode LoadFromCompressedBase64(string aBase64)
        {
            var tmp = System.Convert.FromBase64String(aBase64);
            var stream = new System.IO.MemoryStream(tmp);
            stream.Position = 0;
            return LoadFromCompressedStream(stream);
        }
#else
        public static JSONNode LoadFromCompressedFile(string aFileName)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
        public static JSONNode LoadFromCompressedStream(Stream aData)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
        public static JSONNode LoadFromCompressedBase64(string aBase64)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
#endif

        public static JSONNode LoadFromStream(Stream aData)
        {
            using (var R = new BinaryReader(aData))
            {
                return Deserialize(R);
            }
        }
        public static JSONNode LoadFromFile(string aFileName)
        {
#if USE_FileIO
            using (FileStream F = File.OpenRead(aFileName))
            {
                return LoadFromStream(F);
            }
#else
            throw new Exception("Can't use File IO stuff in webplayer");
#endif
        }
        public static JSONNode LoadFromBase64(string aBase64)
        {
            byte[] tmp = Convert.FromBase64String(aBase64);
            var stream = new MemoryStream(tmp);
            stream.Position = 0;
            return LoadFromStream(stream);
        }
    } // End of JSONNode

    public class JSONArray : JSONNode, IEnumerable
    {
        private readonly List<JSONNode> m_List = new List<JSONNode>();
        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                {
                    return new JSONLazyCreator(this);
                }
                return m_List[aIndex];
            }
            set
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                {
                    m_List.Add(value);
                }
                else
                {
                    m_List[aIndex] = value;
                }
            }
        }
        public override JSONNode this[string aKey]
        {
            get => new JSONLazyCreator(this);
            set => m_List.Add(value);
        }
        public override int Count => m_List.Count;
        public override IEnumerable<JSONNode> Childs
        {
            get
            {
                foreach (JSONNode N in m_List)
                {
                    yield return N;
                }
            }
        }
        public IEnumerator GetEnumerator()
        {
            foreach (JSONNode N in m_List)
            {
                yield return N;
            }
        }
        public override void Add(string aKey, JSONNode aItem)
        {
            m_List.Add(aItem);
        }
        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_List.Count)
            {
                return null;
            }
            JSONNode tmp = m_List[aIndex];
            m_List.RemoveAt(aIndex);
            return tmp;
        }
        public override JSONNode Remove(JSONNode aNode)
        {
            m_List.Remove(aNode);
            return aNode;
        }
        public override string ToString()
        {
            var result = "[ ";
            foreach (JSONNode N in m_List)
            {
                if (result.Length > 2)
                {
                    result += ", ";
                }
                result += N.ToString();
            }
            result += " ]";
            return result;
        }
        public override string ToString(string aPrefix)
        {
            var result = "[ ";
            foreach (JSONNode N in m_List)
            {
                if (result.Length > 3)
                {
                    result += ", ";
                }
                result += "\n" + aPrefix + "   ";
                result += N.ToString(aPrefix + "   ");
            }
            result += "\n" + aPrefix + "]";
            return result;
        }
        public override void Serialize(BinaryWriter aWriter)
        {
            aWriter.Write((byte)JSONBinaryTag.Array);
            aWriter.Write(m_List.Count);
            for (var i = 0; i < m_List.Count; i++)
            {
                m_List[i].Serialize(aWriter);
            }
        }
    } // End of JSONArray

    public class JSONClass : JSONNode, IEnumerable
    {
        private readonly Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();
        public override JSONNode this[string aKey]
        {
            get
            {
                if (m_Dict.ContainsKey(aKey))
                {
                    return m_Dict[aKey];
                }
                else
                {
                    return new JSONLazyCreator(this, aKey);
                }
            }
            set
            {
                if (m_Dict.ContainsKey(aKey))
                {
                    m_Dict[aKey] = value;
                }
                else
                {
                    m_Dict.Add(aKey, value);
                }
            }
        }
        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                {
                    return null;
                }
                return m_Dict.ElementAt(aIndex).Value;
            }
            set
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                {
                    return;
                }
                string key = m_Dict.ElementAt(aIndex).Key;
                m_Dict[key] = value;
            }
        }
        public override int Count => m_Dict.Count;

        public override IEnumerable<JSONNode> Childs
        {
            get
            {
                foreach (KeyValuePair<string, JSONNode> N in m_Dict)
                {
                    yield return N.Value;
                }
            }
        }

        public override IEnumerable<KeyValuePair<string, JSONNode>> PairChilds
        {
            get
            {
                foreach (KeyValuePair<string, JSONNode> N in m_Dict)
                {
                    yield return N;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
            {
                yield return N;
            }
        }

        public override void Add(string aKey, JSONNode aItem)
        {
            if (!string.IsNullOrEmpty(aKey))
            {
                if (m_Dict.ContainsKey(aKey))
                {
                    m_Dict[aKey] = aItem;
                }
                else
                {
                    m_Dict.Add(aKey, aItem);
                }
            }
            else
            {
                m_Dict.Add(Guid.NewGuid().ToString(), aItem);
            }
        }

        public override JSONNode Remove(string aKey)
        {
            if (!m_Dict.ContainsKey(aKey))
            {
                return null;
            }
            JSONNode tmp = m_Dict[aKey];
            m_Dict.Remove(aKey);
            return tmp;
        }
        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_Dict.Count)
            {
                return null;
            }
            KeyValuePair<string, JSONNode> item = m_Dict.ElementAt(aIndex);
            m_Dict.Remove(item.Key);
            return item.Value;
        }
        public override JSONNode Remove(JSONNode aNode)
        {
            try
            {
                KeyValuePair<string, JSONNode> item = m_Dict.Where(k => k.Value == aNode).First();
                m_Dict.Remove(item.Key);
                return aNode;
            }
            catch
            {
                return null;
            }
        }
        public override string ToString()
        {
            var result = "{";
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
            {
                if (result.Length > 2)
                {
                    result += ", ";
                }
                result += "\"" + Escape(N.Key) + "\":" + N.Value;
            }
            result += "}";
            return result;
        }
        public override string ToString(string aPrefix)
        {
            var result = "{ ";
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
            {
                if (result.Length > 3)
                {
                    result += ", ";
                }
                result += "\n" + aPrefix + "   ";
                result += "\"" + Escape(N.Key) + "\" : " + N.Value.ToString(aPrefix + "   ");
            }
            result += "\n" + aPrefix + "}";
            return result;
        }
        public override void Serialize(BinaryWriter aWriter)
        {
            aWriter.Write((byte)JSONBinaryTag.Class);
            aWriter.Write(m_Dict.Count);
            foreach (string K in m_Dict.Keys)
            {
                aWriter.Write(K);
                m_Dict[K].Serialize(aWriter);
            }
        }
    } // End of JSONClass

    public class JSONData : JSONNode
    {
        private string m_Data;
        public JSONData(string aData)
        {
            m_Data = aData;
        }
        public JSONData(float aData)
        {
            AsFloat = aData;
        }
        public JSONData(double aData)
        {
            AsDouble = aData;
        }
        public JSONData(bool aData)
        {
            AsBool = aData;
        }
        public JSONData(int aData)
        {
            AsInt = aData;
        }
        public override string Value
        {
            get => m_Data;
            set => m_Data = value;
        }

        public override string ToString()
        {
            return "\"" + Escape(m_Data) + "\"";
        }
        public override string ToString(string aPrefix)
        {
            return "\"" + Escape(m_Data) + "\"";
        }
        public override void Serialize(BinaryWriter aWriter)
        {
            var tmp = new JSONData("");

            tmp.AsInt = AsInt;
            if (tmp.m_Data == m_Data)
            {
                aWriter.Write((byte)JSONBinaryTag.IntValue);
                aWriter.Write(AsInt);
                return;
            }
            tmp.AsFloat = AsFloat;
            if (tmp.m_Data == m_Data)
            {
                aWriter.Write((byte)JSONBinaryTag.FloatValue);
                aWriter.Write(AsFloat);
                return;
            }
            tmp.AsDouble = AsDouble;
            if (tmp.m_Data == m_Data)
            {
                aWriter.Write((byte)JSONBinaryTag.DoubleValue);
                aWriter.Write(AsDouble);
                return;
            }

            tmp.AsBool = AsBool;
            if (tmp.m_Data == m_Data)
            {
                aWriter.Write((byte)JSONBinaryTag.BoolValue);
                aWriter.Write(AsBool);
                return;
            }
            aWriter.Write((byte)JSONBinaryTag.Value);
            aWriter.Write(m_Data);
        }
    } // End of JSONData

    internal class JSONLazyCreator : JSONNode
    {
        private readonly string m_Key = null;
        private JSONNode m_Node = null;

        public JSONLazyCreator(JSONNode aNode)
        {
            m_Node = aNode;
            m_Key = null;
        }
        public JSONLazyCreator(JSONNode aNode, string aKey)
        {
            m_Node = aNode;
            m_Key = aKey;
        }

        public override JSONNode this[int aIndex]
        {
            get => new JSONLazyCreator(this);
            set
            {
                var tmp = new JSONArray();
                tmp.Add(value);
                Set(tmp);
            }
        }

        public override JSONNode this[string aKey]
        {
            get => new JSONLazyCreator(this, aKey);
            set
            {
                var tmp = new JSONClass();
                tmp.Add(aKey, value);
                Set(tmp);
            }
        }

        public override int AsInt
        {
            get
            {
                var tmp = new JSONData(0);
                Set(tmp);
                return 0;
            }
            set
            {
                var tmp = new JSONData(value);
                Set(tmp);
            }
        }
        public override float AsFloat
        {
            get
            {
                var tmp = new JSONData(0.0f);
                Set(tmp);
                return 0.0f;
            }
            set
            {
                var tmp = new JSONData(value);
                Set(tmp);
            }
        }
        public override double AsDouble
        {
            get
            {
                var tmp = new JSONData(0.0);
                Set(tmp);
                return 0.0;
            }
            set
            {
                var tmp = new JSONData(value);
                Set(tmp);
            }
        }
        public override bool AsBool
        {
            get
            {
                var tmp = new JSONData(false);
                Set(tmp);
                return false;
            }
            set
            {
                var tmp = new JSONData(value);
                Set(tmp);
            }
        }
        public override JSONArray AsArray
        {
            get
            {
                var tmp = new JSONArray();
                Set(tmp);
                return tmp;
            }
        }
        public override JSONClass AsObject
        {
            get
            {
                var tmp = new JSONClass();
                Set(tmp);
                return tmp;
            }
        }

        private void Set(JSONNode aVal)
        {
            if (m_Key == null)
            {
                m_Node.Add(aVal);
            }
            else
            {
                m_Node.Add(m_Key, aVal);
            }
            m_Node = null; // Be GC friendly.
        }
        public override void Add(JSONNode aItem)
        {
            var tmp = new JSONArray();
            tmp.Add(aItem);
            Set(tmp);
        }
        public override void Add(string aKey, JSONNode aItem)
        {
            var tmp = new JSONClass();
            tmp.Add(aKey, aItem);
            Set(tmp);
        }
        public static bool operator ==(JSONLazyCreator a, object b)
        {
            if (b == null)
            {
                return true;
            }
            return ReferenceEquals(a, b);
        }

        public static bool operator !=(JSONLazyCreator a, object b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return true;
            }
            return ReferenceEquals(this, obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "";
        }
        public override string ToString(string aPrefix)
        {
            return "";
        }
    } // End of JSONLazyCreator

    public static class JSON
    {
        public static JSONNode Parse(string aJSON)
        {
            return JSONNode.Parse(aJSON);
        }
    }
}
