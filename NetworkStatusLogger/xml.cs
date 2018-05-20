using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace NetworkStatusLogger
{
    static class xml<type>
    {
        /// <summary>
        /// XMLをクラス変換
        /// </summary>
        /// <param name="stream">シリアライズされたクラス</param>
        /// <returns></returns>
        public static type ReadXml(Stream stream)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(type));
            return (type)serializer.ReadObject(stream);
        }

        /// <summary>
        /// クラスをxml変換
        /// </summary>
        /// <param name="obj">クラスデータ</param>
        /// <returns>シリアライズされた文字列</returns>
        public static string SerializeXml(type obj)
        {
            string data;

            DataContractSerializer deSerializer = new DataContractSerializer(typeof(type));
            using (MemoryStream ms = new MemoryStream())
            {
                deSerializer.WriteObject(ms, obj);
                data = Encoding.UTF8.GetString(ms.ToArray());
            }
            return data;
        }
    }
}
