using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Models.ReportModels;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using static GTI_Mvc.Functions;

namespace GTI_MVC.Controllers {
    public class PlataformaController : Controller
    {
        private readonly string _connection = "GTIconnection";
        [Route("Rod_menu")]
        [HttpGet]
        public ActionResult Rod_menu() {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            int _userid = Convert.ToInt32(Session["hashid"]);
            bool _func = Session["hashfunc"].ToString() == "S";

            List<Rodo_empresa> Lista = tributarioRepository.Lista_Rodo_empresa(_userid,_func);
            ViewBag.Lista_Empresa = new SelectList(Lista,"Codigo","Nome");

            RodoviariaViewModel model = new RodoviariaViewModel();
            return View(model);
        }

        [Route("Rod_menu")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rod_menu(RodoviariaViewModel model) {
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);

            List<Rodo_empresa> Lista = tributarioRepository.Lista_Rodo_empresa();
            ViewBag.Lista_Empresa = new SelectList(Lista,"Codigo","Nome");

            return RedirectToAction("Rod_plat_query",new { a = Encrypt(model.Codigo.ToString()),c = Encrypt(DateTime.Now.Year.ToString()) });
        }

        [Route("Rod_plat_query")]
        [HttpGet]
        public ActionResult Rod_plat_query(string a,string c) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");
            int _codigo, _ano;
            try {
                _codigo = Convert.ToInt32(Decrypt(a));
                _ano = Convert.ToInt32(Decrypt(c));
            } catch(Exception) {
                return RedirectToAction("Login","Home");
            }
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            List<Rodo_uso_plataforma_Struct> Lista = tributarioRepository.Lista_Rodo_uso_plataforma(_codigo,_ano);
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            string _nome = cidadaoRepository.Retorna_Nome_Cidadao(_codigo);
            RodoviariaViewModel model = new RodoviariaViewModel {
                Codigo = _codigo,
                Nome = _nome,
                Lista_uso_plataforma = Lista
            };
            List<AnoList> ListaAno = new List<AnoList>();
            for(int i = 2020;i <= DateTime.Now.Year;i++) {
                AnoList _reg = new AnoList() {
                    Codigo = i,
                    Descricao = i.ToString()
                };
                ListaAno.Add(_reg);
            }

            ViewBag.ListaAno = new SelectList(ListaAno,"Codigo","Descricao",ListaAno[ListaAno.Count - 1].Codigo);
            if(model.Ano == 0)
                model.Ano = DateTime.Now.Year;
            return View(model);
        }

        [Route("Rod_plat_query")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rod_plat_query(RodoviariaViewModel model,string DataDe,string DataAte,string Codigo,string Qtde1,string Qtde2,string Qtde3) {
            //            FormCollection collection=new FormCollection
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            string _name = "";
            int _cod = model.Codigo;
            int _year = model.Ano;
            var data1 = DataDe;
            bool t = DateTime.TryParse(data1,out DateTime _data1);
            t = DateTime.TryParse(DataAte,out DateTime _data2);
            List<Rodo_uso_plataforma_Struct> Lista = tributarioRepository.Lista_Rodo_uso_plataforma(_cod,_year);
            List<AnoList> ListaAno = new List<AnoList>();
            if(DataDe == null) {
                _name = cidadaoRepository.Retorna_Nome_Cidadao(_cod);
                RodoviariaViewModel model2 = new RodoviariaViewModel {
                    Codigo = _cod,
                    Nome = _name,
                    Lista_uso_plataforma = Lista
                };
                for(int i = 2020;i <= DateTime.Now.Year;i++) {
                    AnoList _reg = new AnoList() {
                        Codigo = i,
                        Descricao = i.ToString()
                    };
                    ListaAno.Add(_reg);
                }

                ViewBag.ListaAno = new SelectList(ListaAno,"Codigo","Descricao",ListaAno[ListaAno.Count - 1].Codigo);
                return View(model2);
            }

            var cod = Codigo;
            
            int _codigo = Convert.ToInt32(cod);
            var qtde1 = Qtde1;
            if(string.IsNullOrEmpty(Qtde1)) qtde1 = "0";
            int _qtde1 = Convert.ToInt32(qtde1);
            var qtde2 = Qtde2;
            if(string.IsNullOrEmpty(Qtde2)) qtde2 = "0";
            int _qtde2 = Convert.ToInt32(qtde2);
            var qtde3 = Qtde3;
            if(string.IsNullOrEmpty(Qtde3)) qtde3 = "0";
            int _qtde3 = Convert.ToInt32(qtde3);
            short _ano = (short)_data1.Year;
            int _userId = Convert.ToInt32(Session["hashid"]);
            decimal _valorGuia = 0;

            if(_qtde1==0 && _qtde2==0 && _qtde3 == 0) {
                for (int i = 2020; i <= DateTime.Now.Year; i++) {
                    AnoList _reg = new AnoList() {
                        Codigo = i,
                        Descricao = i.ToString()
                    };
                    ListaAno.Add(_reg);
                }


                ViewBag.ListaAno = new SelectList(ListaAno, "Codigo", "Descricao", ListaAno[ListaAno.Count - 1].Codigo);
                if (model.Ano == 0)
                    model.Ano = DateTime.Now.Year;

                Lista = tributarioRepository.Lista_Rodo_uso_plataforma(_codigo, model.Ano);
                model.Lista_uso_plataforma = Lista;
                return View(model);
            }

            decimal _valor1 = tributarioRepository.Retorna_Valor_Tributo(_ano,154);
            decimal _valor2 = tributarioRepository.Retorna_Valor_Tributo(_ano,155);
            decimal _valor3 = tributarioRepository.Retorna_Valor_Tributo(_ano,156);

            short _seq = tributarioRepository.Retorna_Ultima_Seq_Uso_Plataforma(_codigo,_ano);
            _seq++;
            DateTime _dataVencto = _data2.AddDays(10);

            Exception ex2 = null;
            //grava parcela
            Debitoparcela regParcela = new Debitoparcela {
                Codreduzido = _codigo,
                Anoexercicio = _ano,
                Codlancamento = 52,
                Seqlancamento = _seq,
                Numparcela = 1,
                Codcomplemento = 0,
                Statuslanc = 3,
                Datavencimento = _dataVencto,
                Datadebase = DateTime.Now,
                Userid = _userId
            };
            try {
                ex2 = tributarioRepository.Insert_Debito_Parcela(regParcela);
            } catch(Exception) {

                throw;
            }

            //grava tributo
            if(_qtde1 > 0) {
                decimal _valorTotal1 = _valor1 * _qtde1;
                Debitotributo regTributo = new Debitotributo {
                    Codreduzido = _codigo,
                    Anoexercicio = _ano,
                    Codlancamento = 52,
                    Seqlancamento = _seq,
                    Numparcela = 1,
                    Codcomplemento = 0,
                    Codtributo = (short)154,
                    Valortributo = Math.Round(_valorTotal1,2)
                };
                ex2 = tributarioRepository.Insert_Debito_Tributo(regTributo);
            }
            if(_qtde2 > 0) {
                decimal _valorTotal2 = _valor2 * _qtde2;
                Debitotributo regTributo = new Debitotributo {
                    Codreduzido = _codigo,
                    Anoexercicio = _ano,
                    Codlancamento = 52,
                    Seqlancamento = _seq,
                    Numparcela = 1,
                    Codcomplemento = 0,
                    Codtributo = (short)155,
                    Valortributo = Math.Round(_valorTotal2,2)
                };
                ex2 = tributarioRepository.Insert_Debito_Tributo(regTributo);
            }
            if(_qtde3 > 0) {
                decimal _valorTotal3 = _valor3 * _qtde3;
                Debitotributo regTributo = new Debitotributo {
                    Codreduzido = _codigo,
                    Anoexercicio = _ano,
                    Codlancamento = 52,
                    Seqlancamento = _seq,
                    Numparcela = 1,
                    Codcomplemento = 0,
                    Codtributo = (short)156,
                    Valortributo = Math.Round(_valorTotal3,2)
                };
                ex2 = tributarioRepository.Insert_Debito_Tributo(regTributo);
            }

            //retorna o valor atualizado do débito (lançamento retroativo)
            List<SpExtrato> ListaTributo = tributarioRepository.Lista_Extrato_Tributo(_codigo,_ano,_ano,52,52,_seq,_seq,1,1,0,0,3,3,DateTime.Now,"Web");
            List<SpExtrato> ListaParcela = tributarioRepository.Lista_Extrato_Parcela(ListaTributo);

            foreach(SpExtrato item in ListaParcela) {
                _valorGuia += item.Valortotal;
            }

            if(_dataVencto < DateTime.Now) {
                _dataVencto = DateTime.Now.AddDays(10);
            }

            //grava o documento
            Numdocumento regDoc = new Numdocumento {
                Valorguia = _valorGuia,
                Emissor = "Gti.Web/UsoPlataforma",
                Datadocumento = _dataVencto,
                Registrado = true,
                Percisencao = 0
            };
            regDoc.Percisencao = 0;
            int _novo_documento = tributarioRepository.Insert_Documento(regDoc);

            //grava o documento na parcela
            Parceladocumento regParc = new Parceladocumento {
                Codreduzido = _codigo,
                Anoexercicio = _ano,
                Codlancamento = 52,
                Seqlancamento = _seq,
                Numparcela = 1,
                Codcomplemento = 0,
                Numdocumento = _novo_documento,
                Valorjuros = 0,
                Valormulta = 0,
                Valorcorrecao = 0,
                Plano = 0
            };
            tributarioRepository.Insert_Parcela_Documento(regParc);

            string sHist = "REFERENTE A " + (_qtde1 + _qtde2 + _qtde3).ToString() + " TAXAS DE EMBARQUE DO TERMINAL RODOVIÁRIO DO PERÍODO DE " + _data1.ToString("dd/MM/yyyy") + " À " + _data2.ToString("dd/MM/yyyy") + ".";
            //Incluir a observação da parcela
            Obsparcela ObsReg = new Obsparcela() {
                Codreduzido = _codigo,
                Anoexercicio = _ano,
                Codlancamento = 52,
                Seqlancamento = _seq,
                Numparcela = 1,
                Codcomplemento = 0,
                Obs = sHist,
                Userid = _userId,
                Data = DateTime.Now
            };
            ex2 = tributarioRepository.Insert_Observacao_Parcela(ObsReg);

            //Anexo
            string fileName = "";
            foreach(var file in model.Files) {
                if (file != null) {
                    if (file.ContentLength > 0) {
                        string _guid = Guid.NewGuid().ToString("N");
                        string _path = "~/Files/Plataforma/" + _ano + "/";
                        fileName = _guid + ".pdf";
                        var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(_path), fileName);
                        file.SaveAs(path);
                    }
                }
            }

            //Incluir rodo_uso_plataforma
            short _seq2 = tributarioRepository.Retorna_Ultima_Seq_Uso_Plataforma(_codigo,_data1,_data2);
            // _seq2++;

            Rodo_uso_plataforma regR = new Rodo_uso_plataforma {
                Codigo = _codigo,
                Datade = _data1,
                Dataate = _data2,
                Seq = (byte)_seq2,
                SeqDebito = (byte)_seq,
                Qtde1 = _qtde1,
                Qtde2 = _qtde2,
                Qtde3 = _qtde3,
                Numero_Guia = _novo_documento,
                Valor_Guia = _valorGuia,
                Situacao = 7, //não pago
                Anexo=fileName
            };
            ex2 = tributarioRepository.Insert_Rodo_Uso_Plataforma(regR);

            //Enviar para registrar 
            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(_codigo);
            string _bairro = "", _endereco = "", _compl = "", _cidade = "JABOTICABAL";
            string _cpf_cnpj = string.IsNullOrWhiteSpace(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
            int _cep = 14870000;
            bool _r = _cidadao.EtiquetaC != "S";
            string _nome = _cidadao.Nome;
            _endereco = _r ? _cidadao.EnderecoR : _cidadao.EnderecoC;
            _bairro = _r ? _cidadao.NomeBairroR : _cidadao.NomeBairroC;
            int _numero = _r ? (int)_cidadao.NumeroR : (int)_cidadao.NumeroC;
            _compl = _r ? _cidadao.ComplementoR : _cidadao.ComplementoC;
            _cidade = _r ? _cidadao.NomeCidadeR : _cidadao.NomeCidadeC;
            string _uf = _r ? _cidadao.UfR : _cidadao.UfC;
            _cep = _r ? (int)_cidadao.CepR : (int)_cidadao.CepC;


            Ficha_compensacao_documento ficha = new Ficha_compensacao_documento {
                Nome = _nome,
                Endereco = _endereco.Length > 40 ? _endereco.Substring(0,40) : _endereco,
                Bairro = _bairro.Length > 15 ? _bairro.Substring(0,15) : _bairro,
                Cidade = _cidade.Length > 30 ? _cidade.Substring(0,30) : _cidade,
                Cep = Functions.RetornaNumero(_cep.ToString()) ?? "14870000",
                Cpf = Functions.RetornaNumero(_cpf_cnpj),
                Numero_documento = _novo_documento,
                Data_vencimento = _dataVencto,
                Valor_documento = Convert.ToDecimal(_valorGuia),
                Uf = _uf
            };
            ex2 = tributarioRepository.Insert_Ficha_Compensacao_Documento(ficha);
            ex2 = tributarioRepository.Marcar_Documento_Registrado(_novo_documento);


            for(int i = 2020;i <= DateTime.Now.Year;i++) {
                AnoList _reg = new AnoList() {
                    Codigo = i,
                    Descricao = i.ToString()
                };
                ListaAno.Add(_reg);
            }


            ViewBag.ListaAno = new SelectList(ListaAno,"Codigo","Descricao",ListaAno[ListaAno.Count - 1].Codigo);
            if(model.Ano == 0)
                model.Ano = DateTime.Now.Year;

           Lista = tributarioRepository.Lista_Rodo_uso_plataforma(_codigo,_ano);
            model.Lista_uso_plataforma = Lista;
            model.Qtde1 = 0;
            model.Qtde2 = 0;
            model.Qtde3 = 0;


            return View(model);
            //return Json(new { success = true,responseText = "Dados enviados com sucesso!" },JsonRequestBehavior.AllowGet);
        }

        public ActionResult Rod_uso_plataforma_print(string p1,string p2,string p3,string p4,DateTime p5) {

            bool b = DateTime.TryParseExact(p1,"MM/dd/yyyy hh:mm:ss",CultureInfo.InvariantCulture,DateTimeStyles.None,out DateTime _datade);
            bool c = DateTime.TryParseExact(p2,"MM/dd/yyyy hh:mm:ss",CultureInfo.InvariantCulture,DateTimeStyles.None,out DateTime _dataate);
            int _ano = _datade.Year;
            short _seq = Convert.ToInt16(p3);
            int _codigo = Convert.ToInt32(p4);
            p1 = ""; p2 = ""; p3 = ""; p4 = "";

            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Rodo_uso_plataforma_Struct reg = tributarioRepository.Retorna_Rodo_uso_plataforma(_codigo,_datade,_dataate,_seq);

            short _seqdebito = reg.SeqDebito;
            decimal _aliq1 = tributarioRepository.Retorna_Valor_Tributo(_ano,154);
            decimal _aliq2 = tributarioRepository.Retorna_Valor_Tributo(_ano,155);
            decimal _aliq3 = tributarioRepository.Retorna_Valor_Tributo(_ano,156);

            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(_codigo);
            string _bairro = "", _endereco = "", _compl = "", _cidade = "JABOTICABAL", _nome = "";
            string _cpf_cnpj = string.IsNullOrWhiteSpace(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
            int _userId = Convert.ToInt32(Session["hashid"]);
            bool _r = _cidadao.EtiquetaC != "S";
            _nome = _cidadao.Nome;
            _endereco = _r ? _cidadao.EnderecoR : _cidadao.EnderecoC;
            _bairro = _r ? _cidadao.NomeBairroR : _cidadao.NomeBairroC;
            int _numero = _r ? (int)_cidadao.NumeroR : (int)_cidadao.NumeroC;
            _compl = _r ? _cidadao.ComplementoR : _cidadao.ComplementoC;
            _cidade = _r ? _cidadao.NomeCidadeR : _cidadao.NomeCidadeC;
            string _uf = _r ? _cidadao.UfR : _cidadao.UfC;
            int _cep = _r ? (int)_cidadao.CepR : (int)_cidadao.CepC;

            Numdocumento doc = tributarioRepository.Retorna_Dados_Documento(reg.Numero_Guia);
            DateTime _dataVencto = p5.Date;

            List<SpExtrato> ListaTributo = tributarioRepository.Lista_Extrato_Tributo(_codigo, (short)_ano, (short)_ano, 52, 52, _seqdebito, _seqdebito, 1, 1, 0, 0, 0, 99, _dataVencto, "Web");
            List<SpExtrato> ListaParcela = tributarioRepository.Lista_Extrato_Parcela(ListaTributo);
            
            decimal _valorGuia = 0;
            foreach (SpExtrato item in ListaParcela) {
                _valorGuia += item.Valortotal;
            }

            decimal _vp1 = 0, _vm1 = 0, _vj1 = 0, _vt1 = 0;
            decimal _vp2 = 0, _vm2 = 0, _vj2 = 0, _vt2 = 0;
            decimal _vp3 = 0, _vm3 = 0, _vj3 = 0, _vt3 = 0;

            foreach(SpExtrato item in ListaTributo) {

                switch(item.Codtributo) {
                    case 154:
                        _vp1 = item.Valortributo;
                        _vm1 = item.Valormulta;
                        _vj1 = item.Valorjuros;
                        _vt1 = item.Valortotal;
                        break;
                    case 155:
                        _vp2 = item.Valortributo;
                        _vm2 = item.Valormulta;
                        _vj2 = item.Valorjuros;
                        _vt2 = item.Valortotal;
                        break;
                    case 156:
                        _vp3 = item.Valortributo;
                        _vm3 = item.Valormulta;
                        _vj3 = item.Valorjuros;
                        _vt3 = item.Valortotal;
                        break;
                    default:
                        break;
                }
            }

            string _nosso_numero = "287353200" + reg.Numero_Guia.ToString();
            string _convenio = "2873532";
            //***** GERA CÓDIGO DE BARRAS BOLETO REGISTRADO*****
            DateTime _data_base = Convert.ToDateTime("07/10/1997");
            TimeSpan ts = Convert.ToDateTime(_dataVencto) - _data_base;
            int _fator_vencto = ts.Days;
            string _quinto_grupo = string.Format("{0:D4}",_fator_vencto);
            string _valor_boleto_str = string.Format("{0:0.00}",reg.Valor_Guia);
            _quinto_grupo += string.Format("{0:D10}",Convert.ToInt64(Functions.RetornaNumero(_valor_boleto_str)));
            string _barra = "0019" + _quinto_grupo + string.Format("{0:D13}",Convert.ToInt32(_convenio));
            _barra += string.Format("{0:D10}",Convert.ToInt64(reg.Numero_Guia)) + "17";
            string _campo1 = "0019" + _barra.Substring(19,5);
            string _digitavel = _campo1 + Functions.Calculo_DV10(_campo1).ToString();
            string _campo2 = _barra.Substring(23,10);
            _digitavel += _campo2 + Functions.Calculo_DV10(_campo2).ToString();
            string _campo3 = _barra.Substring(33,10);
            _digitavel += _campo3 + Functions.Calculo_DV10(_campo3).ToString();
            string _campo5 = _quinto_grupo;
            string _campo4 = Functions.Calculo_DV11(_barra).ToString();
            _digitavel += _campo4 + _campo5;
            _barra = _barra.Substring(0,4) + _campo4 + _barra.Substring(4,_barra.Length - 4);
            //**Resultado final**
            string _linha_digitavel = _digitavel.Substring(0,5) + "." + _digitavel.Substring(5,5) + " " + _digitavel.Substring(10,5) + "." + _digitavel.Substring(15,6) + " ";
            _linha_digitavel += _digitavel.Substring(21,5) + "." + _digitavel.Substring(26,6) + " " + _digitavel.Substring(32,1) + " " + Functions.StringRight(_digitavel,14);
            string _codigo_barra = Functions.Gera2of5Str(_barra);
            //**************************************************

            UsoPlataformaReport _usoR = new UsoPlataformaReport() {
                Aliquota1 = _aliq1,
                Aliquota2 = _aliq2,
                Aliquota3 = _aliq3,
                Bairro = _bairro,
                Cidade = _cidade,
                Codigo = _codigo,
                Codigo_Barra = _codigo_barra,
                CpfCnpj = _cpf_cnpj,
                Data_Final = _dataate,
                Data_Inicio = _datade,
                Data_Documento = DateTime.Now,
                Data_Vencimento = _dataVencto,
                Endereco = _endereco + ", " + _numero.ToString() + " " + _compl,
                Linha_Digitavel = _linha_digitavel,Nome = _nome,
                Nosso_Numero = _nosso_numero,
                Numero_Guia = reg.Numero_Guia,
                Qtde1 = reg.Qtde1,
                Qtde2 = reg.Qtde2,
                Qtde3 = reg.Qtde3,
                UF = _uf,
                Valor1J = _vj1,
                Valor1M = _vm1,
                Valor1P = _vp1,
                Valor1T = _vt1,
                Valor2J = _vj2,
                Valor2M = _vm2,
                Valor2P = _vp2,
                Valor2T = _vt2,
                Valor3J = _vj3,
                Valor3M = _vm3,
                Valor3P = _vp3,
                Valor3T = _vt3,
                Valor_Guia = _valorGuia
            };

            //Gera Boleto

            List<UsoPlataformaReport> Lista = new List<UsoPlataformaReport> {
                _usoR
            };
            DataSet Ds = Functions.ToDataSet(Lista);
            ReportDataSource rdsAct = new ReportDataSource("dsUsoPlataforma",Ds.Tables[0]);
            ReportViewer viewer = new ReportViewer();
            viewer.LocalReport.Refresh();
            viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Boleto_Uso_Plataforma.rdlc");
            viewer.LocalReport.DataSources.Add(rdsAct);
            byte[] bytes = viewer.LocalReport.Render("PDF",null,out string mimeType,out string encoding,out string extension,out string[] streamIds,out Warning[] warnings);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = string.Empty;
            Response.AddHeader("content-disposition","attachment; filename= UsoPlataforma" + "." + extension);
            Response.OutputStream.Write(bytes,0,bytes.Length);
            Response.Flush();
            Response.End();
            return null;
        }

        public ActionResult Rod_uso_plataforma_cancel(string p1,string p2,string p3,string p4,string p5) {
            bool b = DateTime.TryParseExact(p1,"MM/dd/yyyy hh:mm:ss",CultureInfo.InvariantCulture,DateTimeStyles.None,out DateTime _datade);
            bool c = DateTime.TryParseExact(p2,"MM/dd/yyyy hh:mm:ss",CultureInfo.InvariantCulture,DateTimeStyles.None,out DateTime _dataate);
            int _ano = _datade.Year;
            short _seq = Convert.ToInt16(p3);
            int _codigo = Convert.ToInt32(p4);
            int _doc = Convert.ToInt32(p5);

            Tributario_bll tributarioRepository = new Tributario_bll(_connection);

            List<DebitoStructure> Lista = tributarioRepository.Lista_Tabela_Parcela_Documento(_doc);
            int _seqdebito = Lista[0].Sequencia_Lancamento;

            Exception ex = tributarioRepository.Alterar_Status_Lancamento(_codigo,(short)_ano,52,(short)_seqdebito,1,0,5);
            ex = tributarioRepository.Alterar_Uso_Plataforma_Situacao(_codigo,_datade,_dataate,_seq,4);

            return RedirectToAction("Rod_plat_query", "Plataforma",new { a = Encrypt(p4),c = Encrypt(_ano.ToString()) });

        }

        public FileResult Rod_uso_plataforma_anexo(string p1,string p2) {

            string _anexo = p2;
            string _ano =(Convert.ToDateTime(p1)).Year.ToString();
            string fullName = Server.MapPath("~");
            fullName = Path.Combine(fullName,"Files");
            fullName = Path.Combine(fullName,"Plataforma");
            fullName = Path.Combine(fullName,_ano);
            fullName = Path.Combine(fullName,_anexo);

            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes,System.Net.Mime.MediaTypeNames.Application.Octet,_anexo);

        }

        byte[] GetFile(string s) {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data,0,data.Length);
            if(br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

    }
}