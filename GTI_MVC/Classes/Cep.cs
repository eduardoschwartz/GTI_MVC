using DotNet.CEP.Search.App;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GTI_Mvc.Classes {
    public class Cep {
        public string CEP { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }

        public static Cep Busca(string cep) {
            var cepObj = new Cep();
            var url = "http://apps.widenet.com.br/busca-cep/api/cep.json?code=" + cep;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            string json = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) 
                using(Stream stream = response.GetResponseStream())
                using(StreamReader reader = new StreamReader(stream)) {
                json = reader.ReadToEnd();
            }

            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            JsonCepObject cepJson = json_serializer.Deserialize<JsonCepObject>(json);

            cepObj.CEP = cepJson.code;
            cepObj.Endereco = cepJson.address;
            cepObj.Bairro = cepJson.district;
            cepObj.Cidade = cepJson.city;
            cepObj.Estado = cepJson.state;
            return cepObj;

        }
        public static Cep Busca_Correio(string cep) {
            var cepObj = new Cep();

            CepSearch _cep = new CepSearch();
            string jsonResult = _cep.GetAddressByCep(cep);
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            try { 
                JsonCepCorreioObject cepJson = json_serializer.Deserialize<JsonCepCorreioObject>(jsonResult);
                cepObj.Endereco = cepJson.Rua;
                cepObj.Bairro = cepJson.Bairro;
                cepObj.CEP = cepJson.Cep;
                cepObj.Cidade = cepJson.Cidade.Substring(0, cepJson.Cidade.Length - 3);
                cepObj.Estado = Functions.StringRight(cepJson.Cidade, 2);
            } catch {
                cepObj.Endereco = "";
                cepObj.Bairro = "";
                cepObj.CEP = "";
                cepObj.Cidade = "";
                cepObj.Estado = "";
            }

            return cepObj;

        }
    }

    public class JsonCepObject {
        public string code { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string address { get; set; }
    }

    public class JsonCepCorreioObject {
        public string Cep { get; set; }
        public string Rua { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
    }


}

