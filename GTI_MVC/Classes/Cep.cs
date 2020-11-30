using DotNet.CEP.Search.App;
using GTI_Bll.Classes;
using GTI_Models.Models;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
           // var url = "https://viacep.com.br/ws/" + cep + "/json/";

            //***************
            cep = Functions.RetornaNumero(cep);
            if (cep.Length < 8) {
                cep = cep.PadLeft(8, '0');
            }
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.AllowAutoRedirect = false;
            //HttpWebResponse ChecaServidor = (HttpWebResponse)request.GetResponse();

            //using (Stream webStream = ChecaServidor.GetResponseStream()) {
            //    if (webStream != null) {
            //        using (StreamReader responseReader = new StreamReader(webStream)) {
            //            string response = responseReader.ReadToEnd();
            //            response = Regex.Replace(response, "[{},]", string.Empty);
            //            response = response.Replace("\"", "");

            //            String[] substrings = response.Split('\n');

            //            int cont = 0;
            //            foreach (var substring in substrings) {
            //                if (cont == 1) {
            //                    string[] valor = substring.Split(":".ToCharArray());
            //                    if (valor[0] == "  erro") {
            //                        cepObj.Endereco = "";
            //                        cepObj.Bairro = "";
            //                        cepObj.CEP = "";
            //                        cepObj.Cidade = "";
            //                        cepObj.Estado = "";
            //                        return cepObj;
            //                    }
            //                }

            //                //Logradouro
            //                if (cont == 1) {
            //                    string[] valor = substring.Split(":".ToCharArray());
            //                    cepObj.CEP = valor[1];
            //                }

            //                if (cont == 2) {
            //                    string[] valor = substring.Split(":".ToCharArray());
            //                    cepObj.Endereco = valor[1];
            //                }

            //                //Complemento
            //                if (cont == 3) {
            //                    string[] valor = substring.Split(":".ToCharArray());
            //                    //                                txtComplemento.Text = valor[1];
            //                }

            //                //Bairro
            //                if (cont == 4) {
            //                    string[] valor = substring.Split(":".ToCharArray());
            //                    cepObj.Bairro = valor[1];
            //                }

            //                //Localidade (Cidade)
            //                if (cont == 5) {
            //                    string[] valor = substring.Split(":".ToCharArray());
            //                    cepObj.Cidade = valor[1];
            //                }

            //                //Estado (UF)
            //                if (cont == 6) {
            //                    string[] valor = substring.Split(":".ToCharArray());
            //                    cepObj.Estado = valor[1];
            //                }

            //                cont++;
            //            }
            //        }
            //    }
            //}


            ////******************
            var url = "http://apps.widenet.com.br/busca-cep/api/cep.json?code=" + cep;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.AutomaticDecompression = DecompressionMethods.GZip;

            string json = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream)) {
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

        public static Cep Busca_CepDB(int Cep) {
            var cepObj = new Cep();
            Endereco_bll enderecoRepository = new Endereco_bll("GTiconnection");
            Cepdb _cep = enderecoRepository.Retorna_CepDB(Cep);
            if (_cep == null) {
                cepObj.Endereco = "";
                cepObj.Bairro = "";
                cepObj.CEP = "";
                cepObj.Cidade = "";
                cepObj.Estado = "";
            } else {
                cepObj.Endereco = _cep.Logradouro;
                cepObj.Bairro = enderecoRepository.Retorna_Bairro(_cep.Uf, _cep.Cidadecodigo, _cep.Bairrocodigo);
                cepObj.CEP = _cep.Cep;
                cepObj.Cidade = enderecoRepository.Retorna_Cidade(_cep.Uf, _cep.Cidadecodigo);
                cepObj.Estado = _cep.Uf;
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

