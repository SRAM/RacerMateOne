using System;

using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace XmlStreamLib
{

	enum TagType
	{
		tagAssembly	= 100,
		tagClass,
		tagField,
		tagInherited,
		tagArray,
		tagTable,
		tagList,
		tagKey,
		tagKeyStart
	}


    public static class XUtil
    {
        public static KeyValuePair<string, object> XAttr(this string key, object value)
        {
            return new KeyValuePair<string, object>(key, value);
        }
    }
    /// <summary>
    ///    Summary description for Class1.
    /// </summary>
    sealed public class XmlStream
    {
		private StringBuilder	build = null;
		private bool			fFormat = false;
		private int				iLevel = -1;


		//
		//
        public XmlStream()
        {
        }



		//!!!!!!!!  PUBLIC METHODS  !!!!!!!!!!!

        FileStream outfile = null;
        //StreamReader infile = null;
        string Pre = "";

        //
        //
        public bool OpenXmlFileOut(string fileName)
        {
            try
            {
                iLevel = 0;
                outfile = new FileStream(fileName, FileMode.Create);
                if (outfile != null)
                {
                    build = new StringBuilder();
                    if (build != null)
                    {
                        BuildXTag("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>", null, null);
                    }
                    return FlushXBuild();
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                CloseXmlFileOut();
            }
            return false;
        }

        public void CloseXmlFileOut()
        {
            FlushXBuild();
            if (outfile != null)
            {
                outfile.Close();
                outfile = null;
                iLevel = -1;
                fFormat = false;
            }
        }

        //
        //
        public bool FlushXBuild()
        {
            MemoryStream strm = null;
            try
            {
                string str = build.ToString();
                build.Remove(0, build.Length);
                //convert string to stream
                if (str != null)
                {
                    strm = new MemoryStream();
                    if (strm != null)
                    {
                        byte[] strmByte = null;
                        strmByte = XmlStreamConverter.ConvertStringToByteArray(str);
                        if (strmByte != null)
                        {
                            strm.Write(strmByte, 0, str.Length);
                        }
                        byte[] bytes = null;
                        bytes = strm.ToArray();
                        if (bytes != null)
                        {
                            outfile.Write(bytes, 0, bytes.Length);
                        }
                        strm.Close();
                        return true;
                    }
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                if (strm != null)
                {
                    strm.Close();
                }
            }
            return false;
        }

        //
        //
        public void ModifyXLevel(int incLevel)
        {
            iLevel += incLevel;
        }

        public void AddXAttrib(ref string atts, string tag, object obj)
        {
            atts += " " + tag + "=\"" + obj + "\"";
        }
        public void AddXElement(string tag, string atts)
        {
            AddXTags("<" + tag + atts + "/>");
        }
        public void AddXElement(string tag, params KeyValuePair<string, object>[] attribs)
        {
            string atts = "";

            foreach (KeyValuePair<string, object> arg in attribs)
            {
                atts += " " + arg.Key + "=\"" +arg.Value + "\"";
            }
            AddXTags("<" + tag + atts + "/>");
        }
        public void AddXElement(string tag, int incLevel)
        {
            AddXTags("<" + tag + "/>", incLevel);
        }
        public void AddXElementStart(string tag, string atts)
        {
            AddXTags("<" + tag + atts + ">");
        }
        public void AddXElementStart(string tag, string atts, int incLevel)
        {
            AddXTags("<" + tag + atts + ">", incLevel);
        }
        public void AddXElementStart(string tag, params KeyValuePair<string, object>[] attribs)
        {
            string atts = "";

            foreach (KeyValuePair<string, object> arg in attribs)
            {
                atts += " " + arg.Key + "=\"" + arg.Value + "\"";
            }
            AddXTags("<" + tag + atts + ">");
        }
        public void AddXElementStart(string tag)
        {
            AddXTags("<" + tag + ">");
        }
        public void AddXElementStart(string tag, int incLevel)
        {
            AddXTags("<" + tag + ">", incLevel);
        }
        public void AddXElementEnd(string tag)
        {
            AddXTags("</" + tag + ">");
        }
        public void AddXElementEnd(string tag, int incLevel)
        {
            AddXTags("</" + tag + ">", incLevel);
        }
        public void AddXField(string tag, object val)
        {
            BuildXTag("<" + tag + ">" + val + "</" + tag + ">", null, null);
        }
        public void AddXValue(object obj, string tagName)
        {
            try
            {
                if(tagName != null)
                    BuildXTag("<" + tagName + ">", null, null);
                StartElement(obj, null, null);
                if (tagName != null)
                    BuildXTag("</" + tagName + ">", null, null);
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
            }
        }
        public bool AddXTags(string str)
        {
            return AddXTags(str, 0);
        }
        public bool AddXTags(string str, int incLevel)
        {
            try
            {
                if (str != null)
                {
                    if (incLevel > 0)
                    {
                        BuildXTag(str, null, null);
                        iLevel += incLevel;
                    }
                    else if (incLevel < 0)
                    {
                        iLevel += incLevel;
                        BuildXTag(str, null, null);
                    }
                    else
                        BuildXTag(str, null, null);
                }
                return true;
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
            }
            return false;
        }


        //
        //
        //
        //
        public bool AddObject(object obj, string tagName)
        {
            try
            {
                BuildXTag("<" + tagName + ">" , null, null);

                StartElement(obj, null, null);

                BuildXTag("</" + tagName + ">", null, null);
                return true;
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
            }
            return false;
        }

        //
        //
        private void StartElement(object obj, Type typeBase, FieldInfo info)
        {
            try
            {
                if (obj != null)
                {
                    Type type = null;
                    //for inherited object from a different assembly
                    if (typeBase != null)
                    {
                        type = typeBase;
                    }
                    else
                    {
                        type = obj.GetType();
                    }
                    if (type != null)
                    {
                        string nameSpace;
                        nameSpace = type.Namespace;
                        if (nameSpace.Length != 0)
                        {
                            StartElementClass(obj, type, info);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ThrowException("Caught exception in XmlStream.FillStream", exc);
            }
        }

        //
        //
        private void StartElementClass(object obj, Type type, FieldInfo info)
        {
            string strClass = null;

            strClass = type.Name;

            MemberInfo[] members = null;
            members = type.GetFields(BindingFlags.Instance |
                                       BindingFlags.Static |
                                       BindingFlags.Public |
                                       BindingFlags.NonPublic);
            if (members.Length != 0)
            {
                //handle collections as a speciality item
                if (type.Namespace == "System.Collections")
                {
                    if (type.Name == "ArrayList")
                    {
                        BuildXArrayList(type, obj);
                    }
                    else if (type.Name == "Hashtable")
                    {
                        BuildXHashtable(type, obj);
                    }
                    else
                    {
                        ThrowException("System.Collections." + type.Name + " is not currently support.", null);
                    }
                }
                else
                {
                    foreach (FieldInfo mem in members)
                    {
                        try
                        {
                            BuildElementField(mem, obj, type);
                        }
                        catch (Exception excTmp)
                        {
                            ThrowException("Exception caught in XmlStream.StartObjectClass.", excTmp);
                        }
                    }

                    if (type.BaseType != null)
                    {
                        if (type.BaseType.Name != "Object")
                        {
                            if (type.BaseType.Namespace == type.Namespace)
                            {
                                StartElementClass(obj, type.BaseType, null);
                            }
                            else
                            {
                                StartElement(obj, type.BaseType, null);
                            }
                        }
                    }
                }
            }
            else
            {
                if (type.BaseType.Name == "Array")
                {
                    BuildElementArray(null, type, obj);
                }
            }
        }

        //
        //
        private bool BuildElementField(FieldInfo mem, object objCnr, Type type)
        {
            bool fRet = false;
            Type typeField = null;
            typeField = mem.FieldType;
            if ((type != null) && (typeField != null))
            {
                object obj = null;
                try
                {
                    obj = type.InvokeMember(mem.Name,
                                             BindingFlags.Default |
                                             BindingFlags.GetField |
                                             BindingFlags.Public |
                                             BindingFlags.Instance |
                                             BindingFlags.Static |
                                             BindingFlags.NonPublic,
                                             null,
                                             objCnr,
                                             new object[] { });
                    if (obj != null)
                    {
                        Type typeInvoked = null;

                        typeInvoked = obj.GetType();
                        if (typeInvoked.IsNested == true)
                        {
                            if (typeInvoked.IsArray)
                            {
                                string name = typeInvoked.Name;
                                BuildElementArray(mem, typeInvoked, obj);
                            }
                            else
                            {
                                iLevel++;
                                BuildXTag("<" + Pre + "" + mem.Name + ">", null, null);
                                if (type.Namespace == typeInvoked.Namespace)
                                {
                                    StartElementClass(obj, typeInvoked, null);
                                }
                                else
                                {
                                    StartElement(obj, null, null);
                                }

                                BuildXTag("</" + Pre + "" + mem.Name + ">", null, null);
                                iLevel--;
                            }
                        }
                        else if (typeInvoked.IsValueType == true)
                        {
                            iLevel++;
                            BuildElementType(mem.Name, typeField.Name, obj);
                            iLevel--;
                        }
                        else if (Type.GetTypeCode(typeInvoked) == TypeCode.String)
                        {
                            iLevel++;
                            BuildElementType(mem.Name, typeField.Name, obj);
                            iLevel--;
                        }
                        else if (typeInvoked.IsClass == true)
                        {
                            if (typeInvoked.IsArray)
                            {
                                string name = typeInvoked.Name;
                                BuildElementArray(mem, typeInvoked, obj);
                            }
                            else
                            {
                                iLevel++;
                                BuildXTag("<" + Pre + "" + mem.Name + ">", null, null);
                                if (type.Namespace == typeInvoked.Namespace)
                                {
                                    StartElementClass(obj, typeInvoked, null);
                                }
                                else
                                {
                                    StartElement(obj, null, null);
                                }

                                BuildXTag("</" + Pre + "" + mem.Name + ">", null, null);
                                iLevel--;
                            }
                        }
                    }

                    fRet = true;
                }
                catch (Exception exc)
                {
                    throw (exc);
                }
            }

            return (fRet);
        }

        //
        //
        private bool BuildElementArray(FieldInfo info, Type type, object obj)
        {
            bool fRet = false;

            if (info != null)
            {
                iLevel++;
                BuildXTag("<" + Pre + "" + info.Name + " Count=\"" + ((Array)obj).Length.ToString() + "\">", null, null);
            }
            foreach (object objElement in ((Array)obj))
            {
                if (objElement != null)
                {
                    Type typeElement = null;

                    typeElement = objElement.GetType();
                    if (typeElement.IsNested == true)
                    {
                        iLevel++;
                        BuildXTag("<" + Pre + "Val>", null, null);

                        if (typeElement.IsArray)
                        {
                            string name = typeElement.Name;
                            BuildElementArray(info, typeElement, objElement);
                        }
                        else
                        {
                            if (type.Namespace == typeElement.Namespace)
                            {
                                StartElementClass(objElement, typeElement, null);
                            }
                            else
                            {
                                StartElement(objElement, null, null);
                            }
                        }

                        BuildXTag("</" + Pre + "Val>", null, null);
                        iLevel--;
                    }
                    else if (typeElement.IsValueType == true)
                    {
                        iLevel++;
                        BuildElementType(null, typeElement.Name, objElement);
                        iLevel--;
                    }
                    else if (Type.GetTypeCode(typeElement) == TypeCode.String)
                    {
                        iLevel++;
                        BuildElementType(null, typeElement.Name, objElement);
                        iLevel--;
                    }
                    else if (type.Namespace == typeElement.Namespace)
                    {
                        StartElementClass(objElement, typeElement, null);
                    }
                    else
                    {
                        StartElement(objElement, null, null);
                    }
                }
            }

            if (info != null)
            {
                BuildXTag("</" + Pre + "" + info.Name + ">", null, null);
                iLevel--;
            }
            fRet = true;

            return (fRet);
        }

        //
        //
        private void BuildElementType(string fieldName, string typeName, object objValue)
        {
            BuildXBegin();

            if (fieldName == null)
                fieldName = "Val";
            build.Append("<" + Pre + "" + fieldName + ">");
            if (typeName == "Char")
            {
                objValue = EncodeCharacter((char)objValue);
            }
            build.Append(objValue.ToString());
            build.Append("</" + Pre + "" + fieldName + ">");

            BuildXEnd();
        }

        //
        //
        private void BuildElementType(string fieldName, string typeName, string strValue)
        {
            BuildXBegin();

            if (fieldName == null)
                fieldName = "";
            build.Append("<" + Pre + "" + fieldName + ">");
            build.Append(strValue);
            build.Append("</" + Pre + "" + fieldName + ">");

            BuildXEnd();
        }









        /*
        //XmlNodeReader nodereader = null;
        //
        //
        public XmlNodeReader OpenNodeReader(string fileName, string xpath)
        {
            XmlNodeReader nr = null;
            try
            {
                StreamReader strm = null;
                strm = new StreamReader(fileName);
                XmlTextReader reader = null;
                reader = new XmlTextReader(strm);
                while (reader.ReadStartElement())
                {
                    if (reader.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (reader.HasValue)
                        writer.Write(reader.Value);

                }

                XmlDocument doc = null;
                doc = new XmlDocument();
                doc.Load(reader);
                XmlNode node = doc.SelectSingleNode(xpath);
                if (node != null)
                {
                    nr = new XmlNodeReader(node);
                }
            }
            catch (Exception exc)
            {
                throw (exc);
            }
            return nr;
        }

        */


        /*
        // Not implemented yet
        //
        public bool OpenXmlFileIn(string fileName)
        {
            try
            {
                //infile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                infile = new StreamReader(fileName);
                if (infile != null)
                {
                    throw new XmlStreamException("Not Implemented.");
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                CloseXmlFileIn();
            }
            return false;
        }

        public void CloseXmlFileIn()
        {
            if (infile != null)
            {
                infile.Close();
                infile = null;
            }
        }

        //
        //
        public XmlDocument ReadXmlDoc(string fileName)
        {
            XmlDocument indoc = null;
            StreamReader strm = null;

            try
            {
                strm = new StreamReader(fileName);
                indoc = ReadXmlStreamDoc(strm);
            }
            catch (Exception exc)
            {
                throw (exc);
            }

            finally
            {
                if (strm != null)
                {
                    strm.Close();
                }
            }
            return (indoc);
        }
        //
        //
        public XmlDocument ReadXmlStreamDoc()
        {
            XmlDocument doc = null;
            try
            {
                XmlTextReader reader = null;
                reader = new XmlTextReader(strm);
                doc = new XmlDocument();
                doc.Load(reader);
            }
            catch (Exception exc)
            {
                throw (exc);
            }
            return doc;
        }

        //
        //
        private object StartRMXBuild(XmlDocument doc)
        {
            object obj = null;
            XmlElement element = null;

            element = doc.DocumentElement;
            if (element != null)
            {
                string strModule = null;
                Assembly assembly = null;

                strModule = GetAttribAssembly(element);
                assembly = (Assembly)Assembly.Load(strModule);
                if (assembly == null)
                {
                    string strType = null;

                    strType = GetAttribType(element);
                    ThrowException("Failed to load " + strModule + " for the " + strType + " assembly.", null);
                }

                XmlNodeList list = null;

                list = doc.DocumentElement.ChildNodes;
                obj = WalkNodeList(list, assembly);
            }

            return (obj);
        }

        */





















        //
        //
        public bool AppendXmlStreamFromObject(object obj)
        {
            try
            {
                string str = null;
                StartObject(obj, null, null);

                str = build.ToString();
                build.Remove(0, build.Length);

                return AddXTags(str);
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
            }
            return false;
        }

        //
        //
        private void StartObject(object obj, Type typeBase, FieldInfo info)
        {
            //string strRet = null;

            try
            {
                if (obj != null)
                {
                    Type type = null;

                    //for inherited object from a different assembly
                    if (typeBase != null)
                    {
                        type = typeBase;
                    }
                    else
                    {
                        type = obj.GetType();
                    }

                    if (type != null)
                    {
                        string nameSpace;

                        nameSpace = type.Namespace;
                        if (nameSpace.Length != 0)
                        {
                            Module module = null;

                            module = type.Module;

                            //iLevel++;
                            //BuildXTag("<assembly_" + nameSpace + " type=\"" + nameSpace + "\" assembly=\"" + module.Name + "\">", null, null);
                            StartObjectClass(obj, type, info);
                            //BuildXTag("</assembly_", nameSpace, null);
                            //iLevel--;
                        }

                        //strRet = build.ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                ThrowException("Caught exception in XmlStream.FillStream", exc);
            }

            //return (strRet);
        }

        //
        //
        private void StartObjectClass(object obj, Type type, FieldInfo info)
        {
            string strClass = null;

            strClass = type.Name;
            iLevel++;
            if (info == null)
            {
                BuildXTag("<object", " type=\"" + type.Namespace + "." + type.Name + "\"", null);
            }
            else
            {
                BuildXTag("<object name=\"", info.Name + "\" type=\"" + type.Namespace + "." + type.Name + "\"", null);
            }

            MemberInfo[] members = null;

            members = type.GetFields(BindingFlags.Instance |
                                       BindingFlags.Static |
                                       BindingFlags.Public |
                                       BindingFlags.NonPublic);
            if (members.Length == 0)
            {
                if (type.BaseType != null)
                {
                    if (type.BaseType.Name == "Array")
                    {
                        members = type.GetFields(BindingFlags.Instance |
                                                   BindingFlags.Static |
                                                   BindingFlags.Public |
                                                   BindingFlags.IgnoreCase |
                                                   BindingFlags.FlattenHierarchy |
                                                   BindingFlags.NonPublic);
                    }
                }
            }

            if (members.Length != 0)
            {
                //handle collections as a speciality item
                if (type.Namespace == "System.Collections")
                {
                    if (type.Name == "ArrayList")
                    {
                        BuildXArrayList(type, obj);
                    }
                    else if (type.Name == "Hashtable")
                    {
                        BuildXHashtable(type, obj);
                    }
                    else
                    {
                        ThrowException("System.Collections." + type.Name + " is not currently support.", null);
                    }

                }
                else
                {
                    foreach (FieldInfo mem in members)
                    {
                        try
                        {
                            BuildXField(mem, obj, type);
                        }
                        catch (Exception excTmp)
                        {
                            ThrowException("Exception caught in XmlStream.StartObjectClass.", excTmp);
                        }
                    }

                    if (type.BaseType != null)
                    {
                        if (type.BaseType.Name != "Object")
                        {
                            //iLevel++;
                            //BuildXTag("<baseClassOf_" + type.Name + ">", null, null);
                            if (type.BaseType.Namespace == type.Namespace)
                            {
                                StartObjectClass(obj, type.BaseType, null);
                            }
                            else
                            {
                                StartObject(obj, type.BaseType, null);
                            }

                            //BuildXTag("</baseClassOf_" + type.Name + ">", null, null);
                            //iLevel--;

                        }
                    }
                }
            }
            else
            {
                if (type.BaseType != null)
                {
                    if (type.BaseType.Name == "Array")
                    {
                        members = type.GetFields(BindingFlags.Instance |
                                                   BindingFlags.Static |
                                                   BindingFlags.Public |
                                                   BindingFlags.IgnoreCase |
                                                   BindingFlags.FlattenHierarchy |
                                                   BindingFlags.NonPublic);
                    }
                }
            }
            BuildXTag("</object" + ">", null, null);
            iLevel--;
        }

        //
        //
        private bool BuildXArrayList(Type type, object obj)
        {
            bool fRet = false;

            BuildXTag("<list count=\"" + ((ArrayList)obj).Count.ToString() + "\">", null, null);
            foreach (object objElement in (ArrayList)obj)
            {
                if (objElement != null)
                {
                    Type typeElement = null;

                    typeElement = objElement.GetType();
                    if (typeElement.IsValueType == true)
                    {
                        iLevel++;
                        BuildXValueType(objElement.ToString(), typeElement.Name, objElement);
                        iLevel--;
                    }
                    else if (Type.GetTypeCode(typeElement) == TypeCode.String)
                    {
                        iLevel++;
                        BuildXValueType(typeElement.Name, typeElement.Name, objElement);
                        iLevel--;
                    }
                    else
                    {
                        iLevel++;
                        //BuildXTag("<objectListMember_" + typeElement.Name + ">", null, null);
                        BuildXTag("<object" + ">", null, null);
                        if (type.Namespace == typeElement.Namespace)
                        {
                            StartObjectClass(objElement, typeElement, null);
                        }
                        else
                        {
                            StartObject(objElement, null, null);
                        }

                        BuildXTag("</object" + ">", null, null);
                        iLevel--;
                    }
                }
            }

            BuildXTag("</list>", null, null);
            fRet = true;
            return (fRet);
        }

        //
        //
        private bool BuildXHashtable(Type type, object obj)
        {
            bool fRet = false;

            BuildXTag("<table count=\"" + ((Hashtable)obj).Count.ToString() + "\">", null, null);
            foreach (object objElement in (Hashtable)obj)
            {
                if (objElement != null)
                {
                    Type typeElement = null;

                    typeElement = objElement.GetType();

                    object objKey = null;
                    object objVal = null;

                    objKey = ((DictionaryEntry)objElement).Key;
                    if (objKey != null)
                    {
                        Type typeKey = null;

                        iLevel++;

                        //deal with the key 1st
                        //if this is an object of some sort
                        //I could be really fucked - but deal with IT!
                        typeKey = objKey.GetType();
                        if (typeKey.IsValueType == true)
                        {
                            BuildXTag("<item" + " key=\"", objKey.ToString() + "\" type=\"" + typeKey.Name + "\"", null);
                        }
                        else if (Type.GetTypeCode(typeKey) == TypeCode.String)
                        {
                            BuildXTag("<item" + " key=\"", objKey + "\" type=\"" + typeKey.Name + "\"", null);
                        }
                        else if (typeKey.IsClass == true)
                        {
                            //this could be a real problem ie. objects as keys
                            if (typeKey.Namespace == typeElement.Namespace)
                            {
                                BuildXTag("<item" + ">", null, null);

                                iLevel++;
                                BuildXTag("<key" + ">", null, null);

                                StartObjectClass(objKey, typeKey, null);
                            }
                            else
                            {
                                BuildXTag("<item" + " namespace=\"" + typeKey.Namespace + "\">", null, null);

                                iLevel++;
                                BuildXTag("<key" + ">", null, null);

                                StartObject(objKey, null, null);
                            }

                            BuildXTag("</key" + ">", null, null);
                            iLevel--;
                        }

                        //now do the value
                        objVal = ((DictionaryEntry)objElement).Value;
                        if (objVal != null)
                        {
                            Type typeVal = null;

                            iLevel++;
                            BuildXTag("<value" + ">", null, null);

                            typeVal = objVal.GetType();
                            if (typeVal.IsValueType == true)
                            {
                                iLevel++;
                                BuildXValueType(objKey.ToString(), typeVal.Name, objVal);
                                iLevel--;
                            }
                            else if (Type.GetTypeCode(typeVal) == TypeCode.String)
                            {
                                iLevel++;
                                BuildXValueType(objKey.ToString(), typeVal.Name, objVal);
                                iLevel--;
                            }
                            else if (typeVal.IsClass == true)
                            {
                                if (typeVal.Namespace == typeElement.Namespace)
                                {
                                    StartObjectClass(objVal, typeVal, null);
                                }
                                else
                                {
                                    StartObject(objVal, null, null);
                                }
                            }

                            BuildXTag("</value" + ">", null, null);
                        }

                        BuildXTag("</item" + ">", null, null);
                        iLevel--;
                    }
                }
            }

            BuildXTag("</table>", null, null);
            fRet = true;
            return (fRet);
        }

        //
        //
        private bool BuildXField(FieldInfo mem, object objCnr, Type type)
        {
            bool fRet = false;
            Type typeField = null;

            typeField = mem.FieldType;
            if ((type != null) && (typeField != null))
            {
                object obj = null;

                try
                {
                    obj = type.InvokeMember(mem.Name,
                                             BindingFlags.Default |
                                             BindingFlags.GetField |
                                             BindingFlags.Public |
                                             BindingFlags.Instance |
                                             BindingFlags.Static |
                                             BindingFlags.NonPublic,
                                             null,
                                             objCnr,
                                             new object[] { });
                    if (obj != null)
                    {
                        Type typeInvoked = null;

                        typeInvoked = obj.GetType();
                        if (typeInvoked.IsClass == true || typeInvoked.IsNested == true)
                        {
                            iLevel++;
                            BuildXTag("<field", " name=\"" + mem.Name + "\"", null);
                            if (typeInvoked.IsArray)
                            {
                                /*
                                //do we require a different assembly?
                                if (type.Namespace != typeInvoked.Namespace)
                                {
                                    iLevel++;
                                    BuildXTag("<assembly_" + typeInvoked.Namespace + " type=\"" + typeInvoked.Namespace + "\" assembly=\"" + typeInvoked.Module.Name + "\">", null, null);
                                }
                                 * */

                                string name = typeInvoked.Name;

                                //iLevel++;
                                //BuildXTag("<class_" + name.Substring(0, name.IndexOf("[]")) + " name=\"", mem.Name + "\" type=\"" + typeInvoked.Namespace + "." + name + "\"", null);
                                BuildXArray(mem, typeInvoked, obj);
                                //BuildXTag("</class_" + name.Substring(0, name.IndexOf("[]")) + ">", null, null);
                                //iLevel--;
                                /*
                                if (type.Namespace != typeInvoked.Namespace)
                                {
                                    BuildXTag("</assembly_" + typeInvoked.Namespace + ">", null, null);
                                    iLevel--;
                                }
                                 * */

                            }
                            else
                            {
                                if (type.Namespace == typeInvoked.Namespace)
                                {
                                    StartObjectClass(obj, typeInvoked, null);
                                }
                                else
                                {
                                    StartObject(obj, null, null);
                                }
                            }
                            BuildXTag("</field" + ">", null, null);
                            iLevel--;
                        }
                        else if (typeInvoked.IsValueType == true)
                        {
                            iLevel++;
                            BuildXValueType(mem.Name, typeField.Name, obj);
                            iLevel--;
                        }
                        else if (Type.GetTypeCode(typeInvoked) == TypeCode.String)
                        {
                            iLevel++;
                            BuildXValueType(mem.Name, typeField.Name, obj);
                            iLevel--;
                        }
                    }

                    fRet = true;
                }
                catch (Exception exc)
                {
                    throw (exc);
                }
            }

            return (fRet);
        }

        //
        //
        private bool BuildXArray(FieldInfo info, Type type, object obj)
        {
            bool fRet = false;

            iLevel++;
            BuildXTag("<array count=\"" + ((Array)obj).Length.ToString() + "\">", null, null);
            foreach (object objElement in ((Array)obj))
            {
                if (objElement != null)
                {
                    Type typeElement = null;

                    typeElement = objElement.GetType();
                    if (typeElement.IsClass == true || typeElement.IsNested == true)
                    {
                        //iLevel++;
                        //BuildXTag("<field", " name=\"" + info.Name + "\"", null);
                        if (typeElement.IsArray)
                        {
                            /*
                            //do we require a different assembly?
                            if (type.Namespace != typeInvoked.Namespace)
                            {
                                iLevel++;
                                BuildXTag("<assembly_" + typeInvoked.Namespace + " type=\"" + typeInvoked.Namespace + "\" assembly=\"" + typeInvoked.Module.Name + "\">", null, null);
                            }
                             * */

                            string name = typeElement.Name;

                            //iLevel++;
                            //BuildXTag("<class_" + name.Substring(0, name.IndexOf("[]")) + " name=\"", mem.Name + "\" type=\"" + typeInvoked.Namespace + "." + name + "\"", null);
                            BuildXArray(info, typeElement, objElement);
                            //BuildXTag("</class_" + name.Substring(0, name.IndexOf("[]")) + ">", null, null);
                            //iLevel--;
                            /*
                            if (type.Namespace != typeInvoked.Namespace)
                            {
                                BuildXTag("</assembly_" + typeInvoked.Namespace + ">", null, null);
                                iLevel--;
                            }
                             * */

                        }
                        else
                        {
                            if (type.Namespace == typeElement.Namespace)
                            {
                                StartObjectClass(objElement, typeElement, null);
                            }
                            else
                            {
                                StartObject(objElement, null, null);
                            }
                        }

                        //BuildXTag("</field" + ">", null, null);
                        //iLevel--;
                    }
                    else if (typeElement.IsValueType == true)
                    {
                        iLevel++;
                        BuildXValueType(null, typeElement.Name, objElement);
                        iLevel--;
                    }
                    else if (Type.GetTypeCode(typeElement) == TypeCode.String)
                    {
                        iLevel++;
                        BuildXValueType(null, typeElement.Name, objElement);
                        iLevel--;
                    }
                    else if (type.Namespace == typeElement.Namespace)
                    {
                        StartObjectClass(objElement, typeElement, null);
                    }
                    else
                    {
                        StartObject(objElement, null, null);
                    }
                }
            }

            BuildXTag("</array>", null, null);
            iLevel--;
            fRet = true;

            return (fRet);
        }

        //
        //
        private void BuildXTag(string strPrefix, string strVariable, string strAttributes)
        {
            BuildXBegin();

            build.Append(strPrefix);
            if (strVariable != null)
            {
                //the assumption being that the prefix will have 
                //a closing bracket if the variable is null
                build.Append(strVariable);
                if (strAttributes != null)
                {
                    build.Append(strAttributes + ">");
                }
                else
                {
                    build.Append(">");
                }
            }

            BuildXEnd();
        }

        //
        //
        private void BuildXValueType(string fieldName, string typeName, object objValue)
        {
            BuildXBegin();

            if(fieldName != null)
                build.Append("<field" + " name=\"" + fieldName + "\"");
            else
                build.Append("<field" + "\"");

            if (typeName != null)
            {
                build.Append(" type=\"" + typeName + "\"");
            }
            build.Append(">");

            if (typeName == "Char")
            {
                objValue = EncodeCharacter((char)objValue);
            }

            build.Append(objValue.ToString());
            build.Append("</field" + ">");

            BuildXEnd();
        }

        //
        //
        private void BuildXValueType(string fieldName, string typeName, string strValue)
        {
            BuildXBegin();

            if (fieldName != null)
                build.Append("<field" + " name=\"" + fieldName + "\"");
            else
                build.Append("<field" + "\"");
            if (typeName != null)
            {
                build.Append(" type=\"" + typeName + "\"");
            }
            build.Append(">");
            build.Append(strValue);
            build.Append("</field" + ">");

            BuildXEnd();
        }

        //
        //
        private void BuildXBegin()
        {
            if (fFormat == true)
            {
                build.Append(new string('\t', iLevel < 0 ? 0 : iLevel));
            }
        }

        //
        //
        private void BuildXEnd()
        {
            if (fFormat == true)
            {
                build.Append("\r\n");
            }
        }















        //
		//
		public Stream CreateXmlStreamFromObject( object obj)
		{
		    MemoryStream strm = null;

			try
			{
				build = new StringBuilder();
				if( build != null)
				{
					iLevel++;
					BuildTag( "<?xml version=\"1.0\" ?>", null, null);
					iLevel--;

				string	str = null;

					str = StartAssembly( obj, null, null);
					//convert string to stream
					if( str != null)
					{
						strm = new MemoryStream();
						if( strm != null)
						{
						byte[] strmByte = null;

							strmByte = XmlStreamConverter.ConvertStringToByteArray( str);
							if( strmByte != null)
							{
								strm.Write(  strmByte, 0, str.Length);
							}
						}
					}
				}
			}
			catch( Exception exc)
			{
				if( strm != null)
				{
					strm.Close();
				}
				throw( exc);
			}

			return( (Stream)strm);
		}

		//
		//
		public FileStream CreateXmlFileFromObject( string fileName, object obj)
		{
		FileStream file = null;

			try
			{
				file = new FileStream( fileName, FileMode.Create);
				if( file != null)
				{
				MemoryStream strm = null;
					strm = (MemoryStream)CreateXmlStreamFromObject( obj);
					if( strm != null)
					{
					byte[] bytes = null;

						bytes = strm.ToArray();
						if( bytes != null)
						{
							file.Write( bytes, 0, bytes.Length);
						}

						strm.Close();
					}

				}
			}
			catch( Exception exc)
			{
				throw( exc);
			}

			finally
			{
				if( file != null)
				{
					file.Close();
				}
			}

			return( file);
		
		}

		//
		//
		public bool Format
		{
			get
			{
				return( fFormat);
			}
			set
			{
				fFormat = value;
			}
		}

		//
		//
        public object CreateObjectFromXmlStream(StreamReader strm)
		{
		object obj = null;

			try
			{
			XmlTextReader reader = null;

				reader = new XmlTextReader( strm);

			XmlDocument doc = null;

				doc = new XmlDocument();
				doc.Load( reader);

				obj = StartDisassembly( doc);
			}
			catch( Exception exc)
			{
				throw( exc);
			}

			return( obj);
		}

		//
		//
		public object CreateObjectFromXmlFile( string fileName)
		{
		object obj = null;
        StreamReader strm = null;

			try
			{
                strm = new StreamReader(fileName);
				obj = CreateObjectFromXmlStream( strm);
			}
			catch( Exception exc)
			{
				throw( exc);
			}

			finally
			{
				if( strm != null)
				{
					strm.Close();
				}
			}
			return( obj);
		}






		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//
		//!!!!!!!!!!!	PRIVATE METHODS	!!!!!!!!!!!!!
		//
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//
		//	Methods to build the XML stream from an obejct
		//
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

		//
		//
		private string StartAssembly( object obj, Type typeBase, FieldInfo info)
		{
		string strRet = null;

			try
			{
				if( obj != null)
				{
				Type type = null;

					//for inherited object from a different assembly
					if( typeBase != null)
					{
						type = typeBase;
					}
					else
					{
						type = obj.GetType();
					}

					if( type != null)
					{
					string nameSpace;

						nameSpace = type.Namespace;
						if( nameSpace.Length != 0)
						{
						Module module = null;

							module = type.Module;

							iLevel++;
							BuildTag( "<assembly_" + nameSpace + " type=\"" + nameSpace + "\" assembly=\"" + module.Name + "\">", null, null);
							StartClass( obj, type, info);
							BuildTag( "</assembly_", nameSpace, null);
							iLevel--;
						}
		
						strRet = build.ToString();
					}
				}
			}
			catch( Exception exc)
			{
				ThrowException( "Caught exception in XmlStream.FillStream", exc);
			}

			return( strRet);
		}

		//
		//
		private void StartClass( object obj, Type type, FieldInfo info)
		{
		string strClass = null;

			strClass = type.Name;
			iLevel++;
			if( info == null)
			{
				BuildTag( "<class_", strClass + " type=\"" + type.Namespace + "." + type.Name + "\"", null);
			}
			else
			{
				BuildTag( "<class_" + strClass + " name=\"", info.Name + "\" type=\"" + type.Namespace + "." + type.Name + "\"", null);
			}

		MemberInfo[] members = null;

			members = type.GetFields( BindingFlags.Instance | 
								       BindingFlags.Static | 
									   BindingFlags.Public | 
									   BindingFlags.NonPublic);
			if( members.Length != 0)
			{
				//handle collections as a speciality item
				if( type.Namespace == "System.Collections")
				{
					if( type.Name == "ArrayList")
					{
						BuildArrayList( type, obj);
					}
					else if( type.Name == "Hashtable")
					{
						BuildHashtable(type, obj);
					}
					else
					{
						ThrowException( "System.Collections." + type.Name + " is not currently support.", null);
					}

				}
				else
				{
					foreach( FieldInfo mem in members)
					{
						try
						{
							BuildField( mem, obj, type);
						}
						catch( Exception excTmp)
						{
							ThrowException( "Exception caught in XmlStream.StartClass.", excTmp);
						}
					}

					if( type.BaseType != null)
					{
						if( type.BaseType.Name != "Object")
						{
							iLevel++;
							BuildTag( "<baseClassOf_" + type.Name + ">", null, null);
							if( type.BaseType.Namespace == type.Namespace)
							{
								StartClass( obj, type.BaseType, null);
							}
							else
							{
								StartAssembly( obj, type.BaseType, null);
							}

							BuildTag( "</baseClassOf_" + type.Name + ">", null, null);
							iLevel--;
							//type = type.BaseType;
						}
					}
				}
			}

			BuildTag( "</class_", strClass, null);
			iLevel--;
		}

		//
		//
		private bool BuildArrayList( Type type, object obj)
		{
		bool fRet = false;

			BuildTag( "<start_List count=\"" + ((ArrayList)obj).Count.ToString() + "\">", null, null);
			foreach( object objElement in (ArrayList)obj)
			{
				if( objElement != null)
				{
				Type typeElement = null;

					typeElement = objElement.GetType();
					if( typeElement.IsValueType == true)
					{
						iLevel++;
						BuildValueType( objElement.ToString(), typeElement.Name, objElement);
						iLevel--;
					}	
					else if( Type.GetTypeCode( typeElement) == TypeCode.String)
					{
						iLevel++;
						BuildValueType( typeElement.Name, typeElement.Name, objElement);
						iLevel--;
					}
					else
					{
						iLevel++;
						BuildTag( "<objectListMember_" + typeElement.Name + ">", null, null);
						if( type.Namespace == typeElement.Namespace)
						{
							StartClass( objElement, typeElement, null);
						}
						else
						{
							StartAssembly( objElement, null, null);
						}

						BuildTag( "</objectListMember_" + typeElement.Name + ">", null, null);
						iLevel--;
					}
				}
			}

			BuildTag( "</start_List>", null, null);
			fRet = true;
			return( fRet);
		}

		//
		//
		private bool BuildHashtable( Type type, object obj)
		{
		bool fRet = false;
	
   			BuildTag( "<start_Table count=\"" + ((Hashtable)obj).Count.ToString() + "\">", null, null);
			foreach( object objElement in (Hashtable)obj)
			{
				if( objElement != null)
				{		
				Type	typeElement = null;

					typeElement = objElement.GetType();

				object objKey = null;
				object objVal = null;
			
					objKey = ((DictionaryEntry)objElement).Key;
					if( objKey != null)
					{
					Type typeKey = null;

						iLevel++;

						//deal with the key 1st
						//if this is an object of some sort
						//I could be really fucked - but deal with IT!
						typeKey = objKey.GetType();
						if( typeKey.IsValueType == true)
						{
							BuildTag( "<item_" + typeKey.Name + " key=\"", objKey.ToString() + "\" type=\"" + typeKey.Name +"\"", null);
						}	
						else if( Type.GetTypeCode( typeKey) == TypeCode.String)
						{
							BuildTag( "<item_" + typeKey.Name + " key=\"", objKey + "\" type=\"" + typeKey.Name + "\"", null);
						}
						else if( typeKey.IsClass == true)
						{
							//this could be a real problem ie. objects as keys
							if( typeKey.Namespace == typeElement.Namespace)
							{
								BuildTag( "<item_" + typeKey.Name + ">", null, null);

								iLevel++;
								BuildTag( "<keyStart_" + typeKey.Name + ">", null, null);

								StartClass( objKey, typeKey, null);
							}
							else
							{
								BuildTag( "<item_" + typeKey.Name + " namespace=\"" + typeKey.Namespace + "\">", null, null);

								iLevel++;
								BuildTag( "<keyStart_" + typeKey.Name + ">", null, null);

								StartAssembly( objKey, null, null);
							}

							BuildTag( "</keyStart_" + typeKey.Name + ">", null, null);
							iLevel--;
						}

						//now do the value
						objVal = ((DictionaryEntry)objElement).Value;
						if( objVal != null)
						{
						Type typeVal = null;

							iLevel++;
							BuildTag( "<valueStart_" + objVal.GetType().Name + ">", null, null);

							typeVal = objVal.GetType();
							if( typeVal.IsValueType == true)
							{
								iLevel++;
								BuildValueType( objKey.ToString(), typeVal.Name, objVal);
								iLevel--;
							}	
							else if( Type.GetTypeCode( typeVal) == TypeCode.String)
							{
								iLevel++;
								BuildValueType( objKey.ToString(), typeVal.Name, objVal);
								iLevel--;
							}
							else if( typeVal.IsClass == true)
							{
								if( typeVal.Namespace == typeElement.Namespace)
								{
									StartClass( objVal, typeVal, null);
								}
								else
								{
									StartAssembly( objVal, null, null);
								}
							}

							BuildTag( "</valueStart_" + objVal.GetType().Name + ">", null, null);
						}

						BuildTag( "</item_" + typeKey.Name + ">", null, null);
						iLevel--;
					}
				}
			}

			BuildTag( "</start_Table>", null, null);
			fRet = true;
			return( fRet);
		}

		//
		//
		private bool BuildField( FieldInfo mem, object objCnr, Type type)
		{
		bool fRet = false;
		Type typeField = null;

			typeField = mem.FieldType;
			if( (type != null) && (typeField != null))
			{
			object obj = null;

				try
				{
					obj = type.InvokeMember( mem.Name, 
										     BindingFlags.Default | 
											 BindingFlags.GetField |
											 BindingFlags.Public |
											 BindingFlags.Instance |
											 BindingFlags.Static |
											 BindingFlags.NonPublic, 
											 null, 
											 objCnr, 
											 new object[] {});
					if( obj != null)
					{
					Type typeInvoked = null;

						typeInvoked = obj.GetType();
						if( typeInvoked.IsNested == true)
						{
							if( typeInvoked.IsArray)
							{
								//do we require a different assembly?
								if( type.Namespace != typeInvoked.Namespace)
								{
									iLevel++;
									BuildTag( "<assembly_" + typeInvoked.Namespace + " type=\"" + typeInvoked.Namespace + "\" assembly=\"" + typeInvoked.Module.Name + "\">", null, null);
								}

							string name = typeInvoked.Name;

								iLevel++;
								BuildTag( "<class_" + name.Substring( 0, name.IndexOf( "[]")) + " name=\"", mem.Name + "\" type=\"" + typeInvoked.Namespace + "." + name + "\"", null);
								BuildArray( mem, typeInvoked, obj);
								BuildTag( "</class_" + name.Substring( 0, name.IndexOf( "[]")) + ">", null, null);
								iLevel--;

								if( type.Namespace != typeInvoked.Namespace)
								{
									BuildTag( "</assembly_" + typeInvoked.Namespace + ">", null, null);
									iLevel--;
								}

							}
							else
							{
								if( type.Namespace == typeInvoked.Namespace)
								{
									StartClass( obj, typeInvoked, mem);
								}
								else
								{
									StartAssembly( obj, null, mem);
								}
							}
						}
						else if( typeInvoked.IsValueType == true)
						{
							iLevel++;
							BuildValueType( mem.Name, typeField.Name, obj);
							iLevel--;
						}	
						else if( Type.GetTypeCode( typeInvoked) == TypeCode.String)
						{
							iLevel++;
							BuildValueType( mem.Name, typeField.Name, obj);
							iLevel--;
						}
						else if( typeInvoked.IsClass == true)
						{
							if( typeInvoked.IsArray)
							{
								//do we require a different assembly?
								if( type.Namespace != typeInvoked.Namespace)
								{
									iLevel++;
									BuildTag( "<assembly_" + typeInvoked.Namespace + " type=\"" + typeInvoked.Namespace + "\" assembly=\"" + typeInvoked.Module.Name + "\">", null, null);
								}

							string name = typeInvoked.Name;

								iLevel++;
								BuildTag( "<class_" + name.Substring( 0, name.IndexOf( "[]")) + " name=\"", mem.Name + "\" type=\"" + typeInvoked.Namespace + "." + name + "\"", null);
								BuildArray( mem, typeInvoked, obj);
								BuildTag( "</class_" + name.Substring( 0, name.IndexOf( "[]")) + ">", null, null);
								iLevel--;

								if( type.Namespace != typeInvoked.Namespace)
								{
									BuildTag( "</assembly_" + typeInvoked.Namespace + ">", null, null);
									iLevel--;
								}

							}
							else
							{
								if( type.Namespace == typeInvoked.Namespace)
								{
									StartClass( obj, typeInvoked, mem);
								}
								else
								{
									StartAssembly( obj, null, mem);
								}
							}
						}
					}

					fRet = true;
				}
				catch( Exception exc)
				{
					throw( exc);
				}
			}

			return( fRet);
		}

		//
		//
		private bool BuildArray( FieldInfo info, Type type, object obj)
		{
		bool	fRet = false;

			iLevel++;
			BuildTag( "<start_Array count=\"" + ((Array)obj).Length.ToString() + "\">", null, null);
			foreach( object objElement in ((Array)obj))
			{
				if( objElement != null)
				{
				Type typeElement = null;

					typeElement = objElement.GetType();

                    if (typeElement.IsNested == true)
                    {
                        if (type.Namespace == typeElement.Namespace)
                        {
                            StartClass(objElement, typeElement, null);
                        }
                        else
                        {
                            StartAssembly(obj, null, null);
                        }

                    }
                    else if (typeElement.IsValueType == true)
					{
						iLevel++;
						BuildValueType( info.Name, typeElement.Name, objElement);
						iLevel--;
					}	
					else if( Type.GetTypeCode( typeElement) == TypeCode.String)
					{
						iLevel++;
						BuildValueType( info.Name, typeElement.Name, objElement);
						iLevel--;
					}
					else if( type.Namespace == typeElement.Namespace)
					{
						StartClass( objElement, typeElement, null);
					}
					else
					{
						StartAssembly( objElement, null, null);
					}
				}
			}

			BuildTag( "</start_Array>", null, null);
			iLevel--;
			fRet = true;
	
			return( fRet);
		}

		//
		//
		private void BuildTag( string strPrefix, string strVariable, string strAttributes)
		{
			BuildBegin();
			
			build.Append( strPrefix);
			if( strVariable != null)
			{
				//the assumption being that the prefix will have 
				//a closing bracket if the variable is null
				build.Append( strVariable);
				if( strAttributes != null)
				{
					build.Append( strAttributes + ">");
				}
				else
				{
					build.Append( ">");
				}
			}

			BuildEnd();
		}

		//
		//
		private void BuildValueType( string fieldName, string typeName, object objValue)
		{
			BuildBegin();

			build.Append( "<field_" + fieldName + " name=\"" + fieldName + "\"");
			if( typeName != null)
			{
				build.Append( " type=\"" + typeName + "\"");
			}
			build.Append( ">");
			if( typeName == "Char")
			{
				objValue = EncodeCharacter( (char)objValue);
			}

			build.Append( objValue.ToString());
			build.Append( "</field_" + fieldName + ">");

			BuildEnd();
		}

		//
		//
		private void BuildValueType( string fieldName, string typeName, string strValue)
		{
			BuildBegin();

			build.Append( "<field_" + fieldName + " name=\"" + fieldName + "\"");
			if( typeName != null)
			{
				build.Append( " type=\"" + typeName + "\"");
			}
			build.Append( ">");
			build.Append( strValue);
			build.Append( "</field_" + fieldName + ">");

			BuildEnd();
		}

		//
		//
		private void BuildBegin()
		{
			if( fFormat == true)
			{
				build.Append( new string( '\t', iLevel)); 
			}
		}

		//
		//
		private void BuildEnd()
		{
			if( fFormat == true)
			{
				build.Append( "\r\n");
			}
		}




		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//
		//   Methods to build object from an xml stream
		//
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


		//
		//
		private object StartDisassembly( XmlDocument doc)
		{
		object		obj = null;
		XmlElement	element = null;

			element = doc.DocumentElement;
			if( element != null)
			{
			string		strModule = null;
			Assembly	assembly = null;

				strModule = GetAttribAssembly( element);
				assembly = (Assembly)Assembly.Load( strModule);
				if( assembly == null)
				{
				string strType = null;

					strType = GetAttribType( element);
					ThrowException( "Failed to load " + strModule + " for the " + strType + " assembly.", null);
				}

			XmlNodeList	list = null;

				list = doc.DocumentElement.ChildNodes;
				obj = WalkNodeList( list, assembly);
			}

			return( obj);
		}

		//
		//
		private object WalkNodeList( XmlNodeList list, params object[] args)
		{
		object		objRet = null;
		Assembly	assembly = null;

			//always assign assembly
			if( args.Length != 0)
			{
				assembly = (Assembly)args[0];
			}

			foreach( XmlElement element in list)
			{
			TagType		tag = 0;
			XmlNodeList	tmpList = null;
			string		strType = null;
			string		strVarName = null;
			object		obj = null;


				tag = GetTagType( element);
				switch( tag)
				{
					case TagType.tagAssembly:
					{
						assembly = LoadAssembly( element);
						tmpList = element.ChildNodes;

						obj = WalkNodeList( tmpList, assembly, args[1]);
						objRet = obj;

						//restore the original assembly
						assembly = (Assembly)args[0];
						break;
					}
					case TagType.tagClass:
					{

						strType = GetAttribType( element);
						strVarName = GetAttribName( element);
						if( (strType.IndexOf( "[]") == -1) &&
							(string.Compare( strType, "System.Collections.Hashtable") != 0) &&
							(string.Compare( strType, "System.Collections.ArrayList") != 0))
						{

							obj = ((Assembly)args[0]).CreateInstance( strType);
							if( obj == null)
							{
								ThrowException( "Failed to create an instance of " + strType + " class.", null);
							}

							objRet = obj;
							//is this a field
							if( strVarName.Length != 0)
							{
								//if so ...
								//1. with the object having been created
								//   assign it to the top level object
								//args[0] is always the object to assign
								AssignObjectToField( args[1], strVarName, obj);
							}

							tmpList = element.ChildNodes;
							WalkNodeList( tmpList, assembly, obj);
						}
						else
						{
						int iIndex = 0;

							//this is an object array
							if( (iIndex = strType.IndexOf( "[]")) != -1)
							{
								obj = CreateObjectArray( assembly, element, strType, strVarName, iIndex);
								AssignObjectToField( args[1], strVarName, obj);
							}
							else if( string.Compare( strType, "System.Collections.ArrayList") == 0)
							{
								obj = CreateArrayList( assembly, element);
								AssignObjectToField( args[1], strVarName, obj);
							}
							else if( string.Compare( strType, "System.Collections.Hashtable") == 0)
							{
								obj = CreateHashtable( assembly, element);
								AssignObjectToField( args[1], strVarName, obj);
							}
						}

						break;
					}
					case TagType.tagField:
					{
						obj = GetElementValue( element);
						if( obj != null)
						{
							//don't assign field for a hashtable
							if( args.Length != 3)
							{
								strVarName = GetAttribName( element);
								AssignObjectToField( args[1], strVarName, obj);
							}
						}

						objRet = obj;
						break;
					}
					case TagType.tagInherited:
					{
						//don't need to create another object
					XmlNode node = null;

						node = element.FirstChild;
						if( node.Name.IndexOf( "class_") == -1)
						{
						Type type = null;

							type = args[1].GetType();
							ThrowException( "Inherited class tag for " + type.Name + " not defined", null);
						}

						tmpList = node.ChildNodes;

						//don't really need the assembly, but it is assigned above
						WalkNodeList( tmpList, assembly, args[1]);
						break;
					}
					case TagType.tagArray:
					{
					
						break;
					}
					case TagType.tagTable:
					{
					
						break;
					}
					case TagType.tagList:
					{
					
						break;
					}
					case TagType.tagKey:
					{
					
						break;
					}
					case TagType.tagKeyStart:
					{
					
						break;
					}
					default:
					{
						ThrowException( "Tag " + ((Int32)(int)tag).ToString() + " type not found", null);
                        break;
					}
				}
			}

			return( objRet);
		}

		//
		//
		private Assembly LoadAssembly( XmlElement element)
		{
		Assembly	assembly = null;
		string		strModule = null;

			strModule = GetAttribAssembly( element);
			assembly = Assembly.Load( strModule);
			if( assembly == null)
			{
			string strType = null;

				strType = GetAttribType( element);
				ThrowException( "Failed to load " + strModule + "for the " + strType + " assembly.", null);
			}

			return( assembly);
		}

		//
		//
		private bool AssignObjectToField( object objAssignTo, string strVarName, object objAssigned)
		{
		bool			fRet = false;
		Type			type = null;

			type = objAssignTo.GetType();

		MemberInfo[]	members = null;

			//get all the fields in the ToAssign object
			members = type.GetFields( BindingFlags.Instance | 
								       BindingFlags.Static | 
									   BindingFlags.Public | 
									   BindingFlags.NonPublic);
			if( members.Length != 0)
			{
				foreach( FieldInfo field in members)
				{
					//find the field to set
					if( string.Compare( field.Name, strVarName) == 0)
					{
						InvokeFieldMember( field, objAssignTo, objAssignTo.GetType(), objAssigned);
					}
				}
		
				fRet = true;
			}

			return( fRet);
		}

		//
		//
		private bool InvokeFieldMember( FieldInfo field, object objAssignTo, Type typeAssignTo, object objAssigned)
		{
		bool fRet = false;

			if( typeAssignTo != null)
			{
			object obj = null;

				try
				{
					obj = typeAssignTo.InvokeMember( field.Name,
													 BindingFlags.Public |
													 BindingFlags.NonPublic |
													 BindingFlags.Instance |
													 BindingFlags.Static |
													 BindingFlags.SetField,
													 null, 
													 objAssignTo, 
													 new object[]{objAssigned});
					fRet = true;
				}
				catch( Exception exc)
				{
					throw( exc);
				}
			}

			return( fRet);
		}

		//
		//
		private object CreateObjectArray( Assembly assembly, XmlElement element, string strType, string strVarName, int iIndex)
		{
		Array	objArray = null;
		string	strTmp = null;

			//drop the "[]"
			strTmp = strType.Substring( 0, iIndex);

		XmlElement	node = null;
		int			iCnt = 0;

			//now get the number of objects
			node = (XmlElement)element.FirstChild;
			iCnt = GetAttribCount( (XmlElement) node);

		object	obj = null;
		Type	type = null;

			//have to create a dummy to get the Type
			obj = assembly.CreateInstance( strTmp);
			if( obj == null)
			{
				ThrowException( "Failed to create an instance of " + strType + " class.", null);
			}

			type = obj.GetType();
			objArray = Array.CreateInstance( type, iCnt);

			//why waste the time screwing around if the array is empty
			if( iCnt > 0)
			{
				node = (XmlElement)node.FirstChild;
				//if this is an array of objects, walk through each object
				if( node.Name.IndexOf( "class_") != -1)
				{
					for( int x = 0; x < iCnt; x++)
					{
					object		objElement = null;
					XmlNodeList list = null;

						objElement = assembly.CreateInstance( strTmp);
						list = node.ChildNodes;
						WalkNodeList( list, assembly, objElement);
						if( objElement == null)
						{
							ThrowException( "Failed to create a " + strType + " as an array element.", null);
						}

						objArray.SetValue( objElement, x);
						node = (XmlElement)node.NextSibling;
					}
				}
				else
				{
					for( int x = 0; x < iCnt; x++)
					{
					object objElement = null;

						objElement = GetElementValue( node);
						if( objElement == null)
						{
							//a char can return a null 
							if( GetAttribType( node) != "Char")
							{
								ThrowException( "An array value, associated with " +  node.Name + " is missing.", null);
							}
						}
						else
						{
							objArray.SetValue( objElement, x);
							node = (XmlElement)node.NextSibling;
						}
					}
				}
			}

			return( objArray);
		}

		//
		//
		public object CreateArrayList( Assembly assembly, XmlElement element)
		{
		ArrayList objArrayList = null;

			try
			{
				objArrayList = new ArrayList();

			XmlNodeList list = null;

				list = element.FirstChild.ChildNodes;
				foreach( XmlElement node in list)
				{
				object obj = null;

					obj = GetElementValue( node);
					if( obj == null)
					{
						//is this an object?
						if( node.Name.IndexOf( "objectListMember_") == -1)
						{
							ThrowException( "ArrayList tag \"objectListMember_\" is misssing.", null);
						}

						obj = GetMemberObject( node, assembly);
					}

					objArrayList.Add( obj);
				}
			}
			catch( Exception exc)
			{
				throw( exc);
			}

			return( objArrayList);
		}

		//
		//
		public object CreateHashtable( Assembly assembly, XmlElement element)
		{
		Hashtable objHashtable = null;

			try
			{
				objHashtable = new Hashtable();

			XmlElement	nodeCount = null;
			int			iCnt = 0;

				nodeCount = (XmlElement)element.FirstChild;
				iCnt = Int32.Parse(nodeCount.GetAttribute( "count"));

			XmlElement node = null;

				//get "item_" tag node
				node = (XmlElement)nodeCount.FirstChild;

				for( int x = 0; x < iCnt; x++)
				{
				string		attrKey = null;
				object		objKey = null;
				object		objValue = null;
				XmlElement	nodeValue = null;
				XmlNodeList list = null;

					attrKey = node.GetAttribute( "type");
					//is the key an object or value type
					if( attrKey.Length == 0)
					{
						//skip past "keyStart_" tag
						list = node.FirstChild.ChildNodes;

						//the "1"s is used a dummy argument so that the field
						//remain unassigned in WalkNodeList method
						objKey = WalkNodeList( list, assembly, 1, 1);

						//get to the value node
						nodeValue = (XmlElement)node.FirstChild.NextSibling;
					}
					else
					{
						objKey = GetAttributeValue( node, "key");
						nodeValue = (XmlElement)node.FirstChild;
					}

					//now get the value portion
					list = nodeValue.ChildNodes;

					//the "1"s is used a dummy argument so that the field
					//remain unassigned in WalkNodeList method
					objValue = WalkNodeList( list, assembly, 1, 1);

					objHashtable.Add( objKey, objValue);
					node = (XmlElement)node.NextSibling;
				}

			}
			catch( Exception exc)
			{
				throw( exc);
			}

			return( objHashtable);
		}

		//
		//
		private TagType GetTagType( XmlElement element)
		{
		TagType tag = 0;

			if( element.Name.IndexOf("assembly_") != -1)
			{
				tag = TagType.tagAssembly;
			}	
			else if( element.Name.IndexOf("class_") != -1)
			{
				tag = TagType.tagClass;
			}	
			else if( element.Name.IndexOf("field_") != -1)
			{
				tag = TagType.tagField;
			}	
			else if( element.Name.IndexOf("baseClassOf_") != -1)
			{
				tag = TagType.tagInherited;
			}	
			else if( element.Name.IndexOf("start_Array") != -1)
			{
				tag = TagType.tagArray;
			}	
			else if( element.Name.IndexOf("start_Table") != -1)
			{
				tag = TagType.tagTable;
			}	
			else if( element.Name.IndexOf("start_List") != -1)
			{
				tag = TagType.tagList;
			}	
			else if( element.Name.IndexOf("itemKey_") != -1)
			{
				tag = TagType.tagKey;
			}	
			else if( element.Name.IndexOf("temValueStart_") != -1)
			{
				tag = TagType.tagKeyStart;
			}	

			return( tag);
		}

		//
		//
		private int GetAttribCount( XmlElement element)
		{
		int iCnt = 0;

			iCnt = Int32.Parse(element.GetAttribute( "count"));
			return( iCnt);
		}

		//
		//
		private string GetAttribType( XmlElement element)
		{
		string type = null;

			type = element.GetAttribute( "type");
			return( type);
		}

		//
		//
		private string GetAttribName( XmlElement element)
		{
		string name = null;

			name = element.GetAttribute( "name");
			return( name);
		}

		//
		//
		private string GetAttribAssembly( XmlElement element)
		{
		string assembly = null;

			assembly = element.GetAttribute( "assembly");
			return( assembly);
		}

		//
		//
		private object GetElementValue( XmlElement element)
		{
		object obj = null;
		string strType = null;
	
			//only do primitive types
			strType = GetAttribType( element);
			if( string.Compare( strType, "Boolean") == 0)
			{
				obj = Boolean.Parse(element.InnerText);
			}
			else if( string.Compare( strType, "Byte") == 0)
			{
				obj = Byte.Parse(element.InnerText);
			}
			else if( string.Compare( strType, "Char") == 0)
			{
				if( string.Compare( element.InnerText, "null") != 0)
				{
					obj = Char.Parse(element.InnerText);
				}
			}
			else if( string.Compare( strType, "DateTime") == 0)
			{
                obj = DateTime.Parse(element.InnerText);
			}
			else if( string.Compare( strType, "Decimal") == 0)
			{
				obj = Decimal.Parse(element.InnerText);
			}
			else if( string.Compare( strType, "Double") == 0)
			{
				obj = Double.Parse(element.InnerText);
			}
			/*
			 * conversion to a GUID will take some thought
			else if( string.Compare( strType, "GUID") == 0)
			{
				obj = element.InnerText;
			}
			*/
			else if( string.Compare( strType, "Int16") == 0)
			{
				obj = Int16.Parse(element.InnerText);
			}
			else if( string.Compare( strType, "Int32") == 0)
			{
				obj = Int32.Parse(element.InnerText);
			}
			else if( string.Compare( strType, "Int64") == 0)
			{
				obj = Int64.Parse(element.InnerText);
			}
			else if( string.Compare( strType, "Sbyte") == 0)
			{
				obj = Char.Parse(element.InnerText);
			}
			else if( string.Compare( strType, "String") == 0)
			{
				obj = element.InnerText;
			}
			else if( string.Compare( strType, "TimeSpan") == 0)
			{
                obj = TimeSpan.Parse(element.InnerText);
			}

			return( obj);
		}

		//
		//
		private object GetAttributeValue( XmlElement element, string attrName)
		{
		object obj = null;
		string strType = null;
	
			//only do primitive types
			strType = GetAttribType( element);
			if( string.Compare( strType, "Boolean") == 0)
			{
				obj = Boolean.Parse(element.GetAttribute( attrName));
			}
			else if( string.Compare( strType, "Byte") == 0)
			{
				obj = Byte.Parse(element.GetAttribute( attrName));
			}
			else if( string.Compare( strType, "Decimal") == 0)
			{
				obj = Decimal.Parse(element.GetAttribute( attrName));
			}
			else if( string.Compare( strType, "Double") == 0)
			{
				obj = Double.Parse(element.GetAttribute( attrName));
			}
			else if( string.Compare( strType, "Int16") == 0)
			{
				obj = Int16.Parse(element.GetAttribute( attrName));
			}
			else if( string.Compare( strType, "Int32") == 0)
			{
				obj = Int32.Parse(element.GetAttribute( attrName));
			}
			else if( string.Compare( strType, "Int64") == 0)
			{
				obj = Int64.Parse(element.GetAttribute( attrName));
			}
			else if( string.Compare( strType, "String") == 0)
			{
				obj = element.GetAttribute( attrName);
			}
			else
			{
				ThrowException( "Element name " + element.Name + " has an attribute type of " + strType + " which is not supported.", null);
			}

			return( obj);
		}

		//
		//
		private object GetMemberObject( XmlElement element, Assembly assembly)
		{
		object		obj = null;
		Assembly	tmpAssembly = null;

			//assign the correct assembly
			if( element.FirstChild.Name.IndexOf( "assembly_") != -1)
			{
				tmpAssembly = LoadAssembly( (XmlElement)element.FirstChild);
			}
			else
			{
				tmpAssembly = assembly;
			}

			if( element.FirstChild.FirstChild.Name.IndexOf( "class_") != -1)
			{
			string strType = null;

				strType = GetAttribType( (XmlElement)element.FirstChild.FirstChild); 
				obj = tmpAssembly.CreateInstance( strType);
			}

			try
			{
				
			}
			catch( Exception exc)
			{
				throw( exc);
			}

			return( obj);
		}

		//
		//
		private string GetValue( XmlElement element)
		{
		string strVal = null;

			strVal = element.Value;
			return( strVal);
		}

		//
		//
		private object EncodeCharacter( char objTest)
		{
		string obj = null;

			switch( objTest)
			{
				case ' ':
				case '&':
				case '<':
				case '>':
				case '\'':
				case '"':
				//copywrite symbol
				case (char) 169:
				//registered symbol
				case (char) 174:
				   obj = "&#" + (Int16)objTest + ";";
					break;
				case (char)0:
					obj = "null";
					break;
				default:
					//do nothing
					obj = objTest.ToString();
					break;
			}

			return( obj);
		}


		//
		//
		private void ThrowException( string msg, Exception e)
		{
		XmlStreamException exc = null;

			if( e == null)
			{
				exc = new XmlStreamException( msg);
			}
			else
			{
				exc = new XmlStreamException( msg, e);
			}

			throw( exc);
		}
    }

    /// <summary>
    ///    Summary description for XmlStreamConverter.
    /// </summary>
    internal class XmlStreamConverter
    {
        internal XmlStreamConverter()
        {
        }

        internal static byte[] ConvertStringToByteArray(string strXml)
        {
            byte[] bytes = null;

            if (strXml != null)
            {
                int iLen = strXml.Length;

                bytes = new byte[iLen];
                if (bytes != null)
                {
                    for (int x = 0; x < iLen; x++)
                    {
                        bytes[x] = (byte)strXml[x];
                    }
                }
            }

            return (bytes);
        }
    }

    /// <summary>
    ///    Summary description for XmlStreamException.
    /// </summary>
    public class XmlStreamException : Exception
    {
        //
        //
        public XmlStreamException()
            : base("An XML stream exception was thrown")
        {
        }

        //
        //
        public XmlStreamException(string msg)
            : base(msg)
        {
        }

        //
        //
        public XmlStreamException(string msg, Exception exc)
            : base(msg, exc)
        {
        }
    }
}
