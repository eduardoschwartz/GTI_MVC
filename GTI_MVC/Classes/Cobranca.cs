using GTI_Models.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Script.Serialization;

namespace GTI_Mvc.Classes {
    public class Cobranca {
        public int numeroConvenio { get; set; }
        public int numeroCarteira { get; set; }
        public int numeroVariacaoCarteira { get; set; }
        public int codigoModalidade { get; set; }
        public string dataEmissao { get; set; }
        public string dataVencimento { get; set; }
        public double valorOriginal { get; set; }
        public double valorAbatimento { get; set; }
        public string codigoAceite { get; set; }
        public int codigoTipoTitulo { get; set; }
        public string indicadorPermissaoRecebimentoParcial { get; set; }
        public string numeroTituloBeneficiario { get; set; }
        public string campoUtilizacaoBeneficiario { get; set; }
        public string numeroTituloCliente { get; set; }
        public string mensagemBloquetoOcorrencia { get; set; }
        public CobrancaPagador pagador { get; set; }
        public string indicadorPix { get; set; }
    }

    public class CobrancaPagador {
        public int tipoInscricao { get; set; }
        public long numeroInscricao { get; set; }
        public string nome { get; set; }
        public string endereco { get; set; }
        public int cep { get; set; }
        public string cidade { get; set; }
        public string bairro { get; set; }
        public string uf { get; set; }
        public string telefone { get; set; }
    }

    public class Cobranca_Retorno {
        public string Erro { get; set; }
        public string Linha_Digitavel { get; set; }
        public string Codigo_Barra { get; set; }
        public string QRCode { get; set; }
        public string Url { get; set; }
        public string Nosso_Numero { get; set; }
        public string txId { get; set; }
        public string Emv { get; set; }
    }

    public class Sistema_Cobranca {
        public static Cobranca_Retorno Registrar_Cobranca(Dam_header dam) {
            Cobranca_Retorno cob = new Cobranca_Retorno() {Erro=""};

            //***********Geração do Token****************
            var client = new RestClient("https://oauth.hm.bb.com.br/oauth/token?gw-dev-app-key=d27b67790cffab50136be17db0050c56b9d1a5b1");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic ZXlKcFpDSTZJalV5TnpoaE16WXRPVEJrTUMwME9UUXhMV0l4WXpVdE1pSXNJbU52WkdsbmIxQjFZbXhwWTJGa2IzSWlPakFzSW1OdlpHbG5iMU52Wm5SM1lYSmxJam95TURjMk5pd2ljMlZ4ZFdWdVkybGhiRWx1YzNSaGJHRmpZVzhpT2pGOTpleUpwWkNJNklqaGhZbVZqTUNJc0ltTnZaR2xuYjFCMVlteHBZMkZrYjNJaU9qQXNJbU52WkdsbmIxTnZablIzWVhKbElqb3lNRGMyTml3aWMyVnhkV1Z1WTJsaGJFbHVjM1JoYkdGallXOGlPakVzSW5ObGNYVmxibU5wWVd4RGNtVmtaVzVqYVdGc0lqb3hMQ0poYldKcFpXNTBaU0k2SW1odmJXOXNiMmRoWTJGdklpd2lhV0YwSWpveE5qSTVNRE0xTlRneE9UY3dmUQ==");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", "cobrancas.boletos-info cobrancas.boletos-requisicao");
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            IRestResponse response = client.Execute(request);

            dynamic responseContent = JsonConvert.DeserializeObject(response.Content);
            string _token = "";
            foreach (dynamic prop in responseContent) {
                string _name = prop.Name.ToString();
                string _value = prop.Value.ToString();
                if (_name == "access_token") {
                    _token = _value;  //<============ Token Gerado
                    break;
                }
            }
            if (_token == "") {//<========= Se o token for inválido retorna erro
           //     return View();
            }

            //**********************************************
            //***********Geração da Cobrança****************
            //**********************************************

            //****Campos variaveis****
            int _numeroConvenio = 3128557;
            string _dataEmissao = DateTime.Now.ToString("dd.MM.yyyy");
            string _dataVencimento = dam.Data_vencimento.ToString("dd.MM.yyyy");
            double _valorOriginal = Math.Round((double)dam.Valor_guia, 2);
            string _numeroTituloBeneficiario = dam.Numero_documento.ToString();
            string _campoUtilizacaoBeneficiario = Functions.RemoveDiacritics("NÃO RECEBER APÓS O VENCIMENTO");
            string _numeroTituloCliente = "000" + _numeroConvenio.ToString() + "00" + _numeroTituloBeneficiario.ToString();
            string _mensagemBloquetoOcorrencia = Functions.RemoveDiacritics("Atualize seus boletos em www.bb.com.br");
            int _tipoInscricao = 1; //(1-Cpf,2-Cnpj)     //─┐
            long _numeroInscricao = 96050176876;         //─┤
            string _nome = "VALERIO DE AGUIAR ZORZATO";  //─┴─ APÓS HOMOLOGAÇÃO TROCAR ESTES 3 CAMPOS POR DADOS REAIS
            string _endereco = Functions.RemoveDiacritics( dam.Endereco);
            int _cep = dam.Cep;
            string _cidade = Functions.RemoveDiacritics(dam.Cidade);
            string _bairro = Functions.RemoveDiacritics(dam.Bairro);
            string _uf = dam.Uf;
            string _telefone = ""; 
            //************************

            client = new RestClient("https://api.hm.bb.com.br/cobrancas/v2/boletos?gw-dev-app-key=d27b67790cffab50136be17db0050c56b9d1a5b1");
            client.Timeout = -1;
            request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + _token); //<===== Informar o token gerado
            request.AddHeader("Content-Type", "application/json");

            var obj = new Cobranca {
                numeroConvenio = _numeroConvenio,
                numeroCarteira = 17,
                numeroVariacaoCarteira = 35,
                codigoModalidade = 1,
                dataEmissao = _dataEmissao,
                dataVencimento = _dataVencimento,
                valorOriginal = _valorOriginal,
                valorAbatimento = 0,
                codigoAceite = "N",
                codigoTipoTitulo = 2,
                indicadorPermissaoRecebimentoParcial = "N",
                numeroTituloBeneficiario = _numeroTituloBeneficiario,
                campoUtilizacaoBeneficiario = _campoUtilizacaoBeneficiario,
                numeroTituloCliente = _numeroTituloCliente,
                mensagemBloquetoOcorrencia = _mensagemBloquetoOcorrencia,
                pagador = new CobrancaPagador {
                    tipoInscricao = _tipoInscricao,
                    numeroInscricao = _numeroInscricao,
                    nome = _nome,
                    endereco = _endereco,
                    cep = _cep,
                    cidade = _cidade,
                    bairro = _bairro,
                    uf = _uf,
                    telefone = _telefone
                },
                indicadorPix = "S"
            };
            var json = new JavaScriptSerializer().Serialize(obj);
            request.AddParameter("application/json", json, RestSharp.ParameterType.RequestBody);
            IRestResponse response2 = client.Execute(request);

            responseContent = JsonConvert.DeserializeObject(response2.Content);
            foreach (dynamic prop in responseContent) {
                string _name = prop.Name.ToString();
                string _value = prop.Value.ToString();
                if (_name == "linhaDigitavel") {
                    cob.Linha_Digitavel = _value;
                }
                if (_name == "codigoBarraNumerico") {
                    cob.Codigo_Barra = _value;
                }
                if (_name == "numero") {
                    cob.Nosso_Numero = _value;
                }
                if (_name == "qrCode") {
                    cob.QRCode = _value;
                    dynamic responseContent2 = JsonConvert.DeserializeObject(_value);
                    foreach (dynamic prop2 in responseContent2) {
                        _name = prop2.Name.ToString();
                        _value = prop2.Value.ToString();
                        if (_name == "url") {
                            cob.Url = _value;
                        }
                        if (_name == "txId") {
                            cob.txId = _value;
                        }
                        if (_name == "emv") {
                            cob.Emv = _value;
                        }
                    }
                }
            }

            return cob;

        }
    }

    public static class Int2of5 {
        private static string Encode(string Data) {
            try {
                //0 = thin
                //1 = thick
                IDictionary<int, string> NumbersMapping = new Dictionary<int, string>();
                NumbersMapping.Add(0, "00110");
                NumbersMapping.Add(1, "10001");
                NumbersMapping.Add(2, "01001");
                NumbersMapping.Add(3, "11000");
                NumbersMapping.Add(4, "00101");
                NumbersMapping.Add(5, "10100");
                NumbersMapping.Add(6, "01100");
                NumbersMapping.Add(7, "00011");
                NumbersMapping.Add(8, "10010");
                NumbersMapping.Add(9, "01010");

                if (string.IsNullOrEmpty(Data)) throw new Exception("No data received");
                if (!Data.All(char.IsDigit)) throw new Exception("Only numbers are accepted");
                if (Data.Length % 2 != 0) throw new Exception("Number os digits have to be even");

                IList<KeyValuePair<int, string>> Digits = new List<KeyValuePair<int, string>>();
                for (int i = 0; i < Data.Length; i++) {
                    int key = Convert.ToInt32(Data[i].ToString());
                    string value = NumbersMapping[Convert.ToInt32(Data[i].ToString())];

                    Digits.Add(new KeyValuePair<int, string>(Convert.ToInt32(Data[i].ToString()),
                               NumbersMapping[Convert.ToInt32(Data[i].ToString())]));
                }

                string Result = string.Empty;
                for (int i = 0; i < Digits.Count; i += 2) {
                    string Pair1 = Digits[i].Value;
                    string Pair2 = Digits[i + 1].Value;

                    //Pair 1 e 2 will get interleaved
                    //Pair 1 = will be bars
                    //Pair 2 = will be spaces
                    //Pseudo-codes:
                    //A = thin space
                    //B = thick space
                    //X = thin bar
                    //Y = thick bar
                    for (int j = 0; j < 5; j++)
                        Result += (Pair1[j].ToString() == "0" ? "X" : "Y") +
                                  (Pair2[j].ToString() == "0" ? "A" : "B");
                }

                //Append start and ending
                return "XAXA" + Result + "YAX";
            } catch (Exception ex) {
                return "#" + ex.Message;
            }
        }

        public static Image GenerateBarCode(string Data, int Width, int Height, int ScaleFactor) {
            try {
                string EncodedData = Encode(Data);
                if (string.IsNullOrEmpty(EncodedData))
                    throw new Exception("Encoding process returned empty");
                if (EncodedData[0].ToString() == "#") throw new Exception(EncodedData);

                int Position = 20, ThinWidth = 1 * ScaleFactor, ThickWidth = 3 * ScaleFactor;
                Image img = new System.Drawing.Bitmap(Width, Height);
                using (Graphics gr = Graphics.FromImage(img)) {
                    //Initial white color filling
                    gr.FillRectangle(Brushes.White, 0, 0, Width, Height);

                    for (int i = 0; i < EncodedData.Length; i++) {
                        //Replace the pseudo-codes with bars or spaces
                        switch (EncodedData[i].ToString()) {
                            case "A":
                                gr.FillRectangle(System.Drawing.Brushes.White,
                                                 Position, 0, ThinWidth, Height);
                                Position += ThinWidth;
                                break;
                            case "B":
                                gr.FillRectangle(System.Drawing.Brushes.White,
                                                 Position, 0, ThickWidth, Height);
                                Position += ThickWidth;
                                break;
                            case "X":
                                gr.FillRectangle(System.Drawing.Brushes.Black,
                                                 Position, 0, ThinWidth, Height);
                                Position += ThinWidth;
                                break;
                            case "Y":
                                gr.FillRectangle(System.Drawing.Brushes.Black,
                                                 Position, 0, ThickWidth, Height);
                                Position += ThickWidth;
                                break;
                        }
                    }
                    return img;
                }
            } catch (Exception ex) {
                throw ex;
            }
        }
    }

}