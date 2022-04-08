using GTI_Bll.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            DateTime Data1 = Convert.ToDateTime("01/10/2021");
            DateTime Data2 = Convert.ToDateTime("03/10/2021");
            Processo_bll processoRepository = new Processo_bll(_connection);

            Print("Buscando Processos: ");
            int _total = 0;
            List<ProcessoAnoNumero> Lista = processoRepository.Lista_Processos_Atraso(Data1, Data2);
            _total = Lista.Count();
            Console.SetCursorPosition(Console.CursorLeft , Console.CursorTop);
            Print("Localizados " + _total.ToString() + " Processos.");
            BreakLine();

            List<int> listaSecretariaRel = new List<int>();
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

                    if (NumDias < 5) goto Proximo;

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
                            listaSecretariaRel.Add(secretaria_codigo);

                    }
Proximo:;
                    _pos++;
                }
            }
            Print("OK");
            BreakLine();
            Print("Secretarias encontradas: " + listaSecretariaRel.Count.ToString());
            BreakLine();

            short _seq = 1;
            for (int z = 0; z < listaSecretariaRel.Count; z++) {
                string _filename =  "REL" + listaSecretariaRel[z].ToString("000") + _seq.ToString("00") + ".TXT";
                string _fullpath = Path.Combine(_path, _filename);
                StreamWriter sw = new StreamWriter(_fullpath);
                sw.WriteLine("Hello World!!" + listaSecretariaRel[z].ToString());
                sw.Close();
            }




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
