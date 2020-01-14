using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Text;

namespace Json2DB
{
    public static class Json2DBFormatParams
    {
        private static string _spModelFolder;
        private static string _spModelFileExt = ".xml";

        private static bool _isSQL = true;
        public static bool IsSQL
        {
            get
            {
                return _isSQL;
            }
            set
            {
                _isSQL = value;
            }
        }
        public static string spModelFolder
        {
            get
            {
                return _spModelFolder;
            }
            set
            {
                _spModelFolder = value;
            }
        }

        public static void ConvertJsonToParams(ref dynamic dbCommand, JObject input, string spName, string clientid)
        {
            string FilePath = spModelFolder + clientid + "\\" + spName + _spModelFileExt;
            DataSet ds = new DataSet();
            ds.ReadXml(FilePath);
            DataTable dt = new DataTable();
            if (ds.Tables.Count > 0)
                dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                if (dt.Columns.Contains("ParameterName"))
                {
                    string ParameterName = "@" + Convert.ToString(dr["ParameterName"]);
                    string valueName = Convert.ToString(dr["valueName"]);

                    string objType = string.Empty;

                    if (dt.Columns.Contains("objType"))
                        objType = Convert.ToString(dr["objType"]);

                    dynamic convertedValue;

                    if (string.IsNullOrEmpty(objType))
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(dr["DbType"])))
                            convertedValue = ConvertType(Convert.ToString(dr["DbType"]), input[valueName]);
                        else
                            convertedValue = Convert.ToString(input[valueName]);
                    }
                    else
                    {
                        if (objType.ToUpper().EndsWith("_SINGLE"))
                            convertedValue = GetSingleXMLFormat(ds, objType, valueName, (JObject)input[valueName]);
                        else if (objType.ToUpper().EndsWith("_MULTI"))
                            convertedValue = GetXMLFormat(ds, objType, valueName, (JArray)input[valueName]);
                        else
                            convertedValue = Convert.ToString(input[valueName]);
                    }


                    if (Convert.ToString(dr["AddWithValue"]) == "1")
                    {
                        dbCommand.Parameters.AddWithValue(ParameterName, convertedValue);
                    }
                    else
                    {
                        dynamic d = GetDBTypeObject();
                        d = GetDbType(Convert.ToString(dr["DbType"]).ToLower());
                        dbCommand.Parameters.Add(ParameterName, d);
                        if (Convert.ToString(dr["Direction"]).Equals("Out"))
                            dbCommand.Parameters[ParameterName].Direction = ParameterDirection.Output;
                    }
                }

            }
        }

        public static void ConvertJsonToParams2(ref dynamic dbCommand, JObject input, string spName, string clientid)
        {
            string FilePath = spModelFolder + clientid + "\\" + _spModelFolder + _spModelFileExt;
            DataSet ds = new DataSet();
            ds.ReadXml(FilePath);
            DataTable dt = new DataTable();
            if (ds.Tables.Count > 0)
                dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                if (dt.Columns.Contains("ParameterName"))
                {
                    string ParameterName = "@" + Convert.ToString(dr["ParameterName"]);
                    string valueName = Convert.ToString(dr["valueName"]);

                    string objType = string.Empty;

                    if (dt.Columns.Contains("objType"))
                        objType = Convert.ToString(dr["objType"]);

                    dynamic convertedValue;

                    if (string.IsNullOrEmpty(objType))
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(dr["DbType"])))
                            convertedValue = ConvertType(Convert.ToString(dr["DbType"]), input[valueName]);
                        else
                            convertedValue = Convert.ToString(input[valueName]);
                    }
                    else
                    {
                        if (objType.ToUpper().EndsWith("_SINGLE"))
                            convertedValue = GetSingleXMLFormat(ds, objType, valueName, (JObject)input[valueName]);
                        else if (objType.ToUpper().EndsWith("_MULTI"))
                            convertedValue = GetXMLFormat(ds, objType, valueName, (JArray)input[valueName]);
                        else
                            convertedValue = Convert.ToString(input[valueName]);
                    }


                    if (Convert.ToString(dr["AddWithValue"]) == "1")
                    {
                        dbCommand.Parameters.AddWithValue(ParameterName, convertedValue);
                    }
                    else
                    {
                        dynamic d = GetDBTypeObject();
                        d = GetDbType(Convert.ToString(dr["DbType"]).ToLower());
                        dbCommand.Parameters.Add(ParameterName, d);
                        if (Convert.ToString(dr["Direction"]).Equals("Out"))
                            dbCommand.Parameters[ParameterName].Direction = ParameterDirection.Output;
                    }
                }

            }
        }

        private static dynamic GetDBTypeObject()
        {
            if (_isSQL)
                return new SqlDbType();
            else
                return new MySqlDbType();
        }

        private static dynamic GetDBTypeEnum(dynamic d,string dbType)
        {
            if(_isSQL)
            {
                switch (dbType)
                {
                    case "int64":
                        d = SqlDbType.Int;
                        break;
                    case "int32":
                        d = SqlDbType.Int;
                        break;
                    case "int24":
                        d = SqlDbType.Int;
                        break;
                    case "int16":
                        d = SqlDbType.Int;
                        break;
                    case "string":
                        d = SqlDbType.NVarChar;
                        break;
                    case "datetime":
                        d = SqlDbType.DateTime;
                        break;
                    case "boolean":
                        d = SqlDbType.Bit;
                        break;
                    case "json":
                        d = SqlDbType.NVarChar;
                        break;
                    case "text":
                        d = SqlDbType.Text;
                        break;
                    case "longtext":
                        d = SqlDbType.NVarChar;
                        break;
                    case "decimal":
                        d = SqlDbType.Decimal;
                        break;
                    case "float":
                        d = SqlDbType.Float;
                        break;
                    case "double":
                        d = SqlDbType.Real;
                        break;
                    case "varchar":
                        d = SqlDbType.VarChar;
                        break;
                    case "bit":
                        d = SqlDbType.Bit;
                        break;
                    default:
                        break;
                }
                return d;
            }
            else
            {
                switch (dbType)
                {
                    case "int64":
                        d = MySqlDbType.Int64;
                        break;
                    case "int32":
                        d = MySqlDbType.Int32;
                        break;
                    case "int24":
                        d = MySqlDbType.Int24;
                        break;
                    case "int16":
                        d = MySqlDbType.Int16;
                        break;
                    case "string":
                        d = MySqlDbType.String;
                        break;
                    case "datetime":
                        d = MySqlDbType.DateTime;
                        break;
                    case "boolean":
                        d = MySqlDbType.Bit;
                        break;
                    case "json":
                        d = MySqlDbType.JSON;
                        break;
                    case "text":
                        d = MySqlDbType.Text;
                        break;
                    case "longtext":
                        d = MySqlDbType.LongText;
                        break;
                    case "decimal":
                        d = MySqlDbType.Decimal;
                        break;
                    case "float":
                        d = MySqlDbType.Float;
                        break;
                    case "double":
                        d = MySqlDbType.Double;
                        break;
                    case "varchar":
                        d = MySqlDbType.VarChar;
                        break;
                    case "bit":
                        d = MySqlDbType.Bit;
                        break;
                    default:
                        break;
                }
                return d;
            }
        }

        private static dynamic GetDbType(string dbType)
        {
            dynamic d = GetDBTypeObject();
            return GetDBTypeEnum(d, dbType);
        }

        private static dynamic ConvertType(string dbType, dynamic value)
        {
            dynamic convertedValue;
            try
            {

                if (!String.IsNullOrEmpty(dbType))
                    switch (Convert.ToString(dbType))
                    {
                        case "Int64":
                            convertedValue = Convert.ToInt64(value);
                            break;
                        case "Int16":
                            convertedValue = Convert.ToInt16(value);
                            break;
                        case "Int32":
                            convertedValue = Convert.ToInt32(value);
                            break;
                        case "String":
                            convertedValue = Convert.ToString(value);
                            break;
                        case "DateTime":
                            convertedValue = Convert.ToDateTime(value);
                            break;
                        case "Boolean":
                            convertedValue = Convert.ToBoolean(value);
                            break;
                        default:
                            convertedValue = Convert.ToString(value);
                            break;
                    }
                else
                    convertedValue = Convert.ToString(value);
            }
            catch (Exception)
            {
                convertedValue = Convert.ToString(value);
            }
            return convertedValue;
        }

        private static string GetXMLFormat(DataSet ds, string tableName, string objName, JArray j)
        {
            StringBuilder str = new StringBuilder();
            DataTable dt = ds.Tables[tableName];


            str.Append("<" + objName + "_list>");


            foreach (var item in j)
            {
                str.Append("<" + objName + ">");
                foreach (DataRow dr in dt.Rows)
                {
                    string valueName = Convert.ToString(dr["valueName"]);
                    string paramName = Convert.ToString(dr["ParameterName"]);
                    string dbType = Convert.ToString(dr["DbType"]);
                    string objType = string.Empty;
                    if (dt.Columns.Contains("objType"))
                        objType = Convert.ToString(dr["objType"]);

                    if (!string.IsNullOrEmpty(objType))
                    {
                        if (objType.ToUpper().EndsWith("_SINGLE"))
                            item[valueName] = GetSingleXMLFormat(ds, objType, valueName, (JObject)j[valueName]);
                        else if (objType.ToUpper().EndsWith("_MULTI"))
                            item[valueName] = GetXMLFormat(ds, objType, valueName, (JArray)item[valueName]);
                        else if (!string.IsNullOrEmpty(dbType))
                            item[valueName] = ConvertType(dbType, item[valueName]);
                        //else
                        //    item[valueName] = item[valueName]; 
                    }


                    //if (string.IsNullOrEmpty(objType))
                    //{
                    str.AppendLine("<field name=\"" + paramName + "\"><![CDATA[" + item[valueName] + "]]></field>");
                    //}
                }
                str.Append("</" + objName + ">");
            }
            str.AppendLine("</" + objName + "_list>");
            return str.ToString();
        }

        private static string GetSingleXMLFormat(DataSet ds, string tableName, string objName, JObject j)
        {
            StringBuilder str = new StringBuilder();
            DataTable dt = ds.Tables[tableName];

            str.Append("<" + objName + ">");
            foreach (DataRow dr in dt.Rows)
            {
                string valueName = Convert.ToString(dr["valueName"]);
                string paramName = Convert.ToString(dr["ParameterName"]);
                string dbType = Convert.ToString(dr["DbType"]);
                string objType = string.Empty;
                if (dt.Columns.Contains("objType"))
                    objType = Convert.ToString(dr["objType"]);

                dynamic convertedValue;
                if (!string.IsNullOrEmpty(objType))
                {
                    if (objType.ToUpper().EndsWith("_SINGLE"))
                        convertedValue = GetSingleXMLFormat(ds, objType, valueName, (JObject)j[valueName]);
                    else if (objType.ToUpper().EndsWith("_MULTI"))
                        convertedValue = GetXMLFormat(ds, objType, valueName, (JArray)j[valueName]);
                    else if (!string.IsNullOrEmpty(dbType))
                        convertedValue = ConvertType(dbType, j[valueName]);
                    else
                        convertedValue = Convert.ToString(j[valueName]);
                }
                else
                    convertedValue = Convert.ToString(j[valueName]);

                if (string.IsNullOrEmpty(objType))
                {
                    str.AppendLine("<field name='" + paramName + "'><![CDATA[" + convertedValue + "]]></field>");
                }
            }
            str.Append("</" + objName + ">");

            return str.ToString();
        }

    }
}
