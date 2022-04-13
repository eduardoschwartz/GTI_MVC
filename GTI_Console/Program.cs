using GTI_Bll.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace GTI_Console {
    class Program {
        private static void Print(string Msg) {
            Console.Write(Msg);
        }

        private static void BreakLine() {
            Print("\r\n");
        }

        static void Main(string[] args) {
            string _connection = gtiCore.Connection_Name();
            string _path = "C:\\WORK\\GTI\\PROCESSO_EMAIL\\" + DateTime.Now.Year.ToString();
            DateTime Data1 = Convert.ToDateTime("01/01/2020");
            DateTime Data2 = Convert.ToDateTime(DateTime.Now.Date);
            Processo_bll processoRepository = new Processo_bll(_connection);

            Print("Buscando Processos: ");
            int _total = 0;
            List<ProcessoAnoNumero> Lista = processoRepository.Lista_Processos_Atraso(Data1, Data2);
            _total = Lista.Count();
            Console.SetCursorPosition(Console.CursorLeft , Console.CursorTop);
            Print("Localizados " + _total.ToString() + " Processos.");
            BreakLine();

            List<short> listaSecretariaRel = new List<short>();
            int _pos = 0;
            List<Local_Tramite> _listaProcessos = new List<Local_Tramite>();
            
            Print("Carregando: ");
            using (var progress = new ProgressBar()) {
                foreach (ProcessoAnoNumero item in Lista) {
                    progress.Report((double)_pos / _total);
                    short _ano = item.Ano;
                    int _numero = item.Numero;

                    Local_Tramite lt = processoRepository.Verificar_Processo(_ano, _numero);
                    DateTime? _data = lt.Data_Evento;
                    if (_data == DateTime.MinValue) {
                        _data = lt.Data_Entrada;
                    }

                    int Local_Codigo = lt.Local_Codigo;
                    DateTime Data_Evento = Convert.ToDateTime(_data);
                    bool Arquivado = lt.Arquivado;
                    bool Suspenso = lt.Suspenso;
                    string _assunto = lt.Assunto_Nome;
                    int NumDias = lt.Dias;

                    if (NumDias < 6) goto Proximo;

                    if (!Arquivado && !Suspenso) {
                        Tuple<short, string> Secretaria = processoRepository.Retorna_Vinculo_Top_CentroCusto((short)Local_Codigo);
                        int secretaria_codigo = Secretaria.Item1;
                        string secretaria_nome = Secretaria.Item2;

                        Local_Tramite reg = new Local_Tramite() {
                            Ano = _ano,
                            Numero = _numero,
                            Secretaria_Codigo = secretaria_codigo,
                            Secretaria_Nome = secretaria_nome,
                            Local_Codigo = Local_Codigo,
                            Local_Nome = lt.Local_Nome,
                            Data_Evento = Data_Evento,
                            Dias = NumDias,
                            Assunto_Nome = _assunto
                        };
                        _listaProcessos.Add(reg);

                        bool _find = false;
                        for (int i = 0; i < listaSecretariaRel.Count; i++) {
                            if (listaSecretariaRel[i] == secretaria_codigo) {
                                _find = true;
                                break;
                            }
                        }
                        if (!_find)
                            listaSecretariaRel.Add((short)secretaria_codigo);

                    }
Proximo:;
                    _pos++;
                }
            }
            Print("OK");
            BreakLine();
            Print("Secretarias encontradas: " + listaSecretariaRel.Count.ToString());
            BreakLine();
            Console.WriteLine("ENVIANDO E-MAILS PARA AS SECRETARIAS");
            Console.WriteLine("------------------------------------");


            for (int z = 0; z < listaSecretariaRel.Count; z++) {
                Secretaria _secretaria = processoRepository.Retorna_Secretaria(listaSecretariaRel[z]);
                Console.WriteLine( _secretaria.Nome);
                int _qtde = 0;
                short _seq = processoRepository.Retorna_Seq_Processo_Secretaria_Remessa(listaSecretariaRel[z]);
                string _filename =  "REL" + listaSecretariaRel[z].ToString("000") + _seq.ToString("00") + ".TXT";
                string _fullpath = Path.Combine(_path, _filename);
                StreamWriter sw = new StreamWriter(_fullpath);
                sw.WriteLine("RELATÓRIO DE PROCESSOS QUE SE ENCONTRAM A MAIS DE 5 DIAS NA SECRETARIA");
                sw.WriteLine("");
                sw.WriteLine( _secretaria.Nome);
                sw.WriteLine("");
                
                sw.WriteLine("Nº PROCESSO  LOCAL ONDE O PROCESSO DE ENCONTRA        ASSUNTO DO PROCESSO                      DIAS");
                sw.WriteLine("===================================================================================================");
                sw.WriteLine("");
                foreach (Local_Tramite item in _listaProcessos.Where(p=>p.Secretaria_Codigo==listaSecretariaRel[z]).OrderBy(h=>h.Local_Nome).ThenByDescending(m=>m.Dias)) {
                    string _processo = item.Numero.ToString("00000") + "-" + processoRepository.DvProcesso(item.Numero) + "/" + item.Ano.ToString();
                    sw.WriteLine(_processo + " " +  gtiCore.TruncateTo( item.Local_Nome.PadRight(40),40) + " " + gtiCore.TruncateTo(item.Assunto_Nome.PadRight(40), 40) + "  " + item.Dias.ToString("000") );
                    _qtde++;
                }

                sw.WriteLine("");
                sw.WriteLine("===============================");
                sw.WriteLine("QTDE DE PROCESSOS ==> " + _qtde.ToString());
                sw.WriteLine("RELATÓRIO GERADO EM " + DateTime.Now);
                sw.WriteLine("GESTÃO DE TIBUTAÇÃO MUNICIPAL INTEGRADA (G.T.I.)");

                sw.Close();

                //Enviar Email
                MailAddress from = new MailAddress("gti@jaboticabal.sp.gov.br", "Sistema GTI");
                MailAddress to = new MailAddress("eduardo.schwartz@gmail.com", "Eduardo");
                using (MailMessage emailMessage = new MailMessage()) {
                    string Body = File.ReadAllText("C:\\WORK\\GTI\\PROCESSO_EMAIL\\AccessTemplate.htm");
                    Body = Body.Replace("#$$$#", _secretaria.Nome);
                    emailMessage.From = from;
                    emailMessage.To.Add(to);
                    emailMessage.Attachments.Add(new Attachment(_fullpath));
                    emailMessage.Subject = "Relatório dos processos quem encontram-se na secretaria a mais de 5 dias";
                    emailMessage.Body = Body;
                    emailMessage.IsBodyHtml = true;

                    using (SmtpClient MailClient = new SmtpClient("smtp.gmail.com", 587)) {
                        MailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        MailClient.EnableSsl = true;
                        MailClient.Credentials = new NetworkCredential("gti.jaboticabal@gmail.com", "esnssgzxxjcdjrpk");
                        MailClient.Send(emailMessage);
                    }
                }


                //Gravar remessa na tabela
                Secretaria_processo_remessa reg = new Secretaria_processo_remessa() {
                    Codigo= listaSecretariaRel[z],
                    Data=DateTime.Now.Date,
                    Seq=_seq,
                    Qtde=_qtde
                };
                Exception ex = processoRepository.Incluir_Secretaria_Processo_Remessa(reg);
            }

            Console.WriteLine("");
            Console.WriteLine("Processo finalizado, aperte uma tecla para finalizar");
            Console.ReadLine();
        }

    }
}



//            Console.Clear();
//            Console.WriteLine("Digite a Data Inicial: ");
//            Console.SetCursorPosition(23, Console.CursorTop -1);
//            string _dataInicial = Console.ReadLine();
//            if (_dataInicial.Length < 10 || !gtiCore.IsDate(_dataInicial)) {
//                BreakLine();
//                Print("Data inicial inválida (formato: dd/mm/yyyy)");
//                Console.ReadLine();
