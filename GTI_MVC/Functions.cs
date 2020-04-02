using GTI_Models;
using GTI_Models.Models;
using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace GTI_Mvc {
    public static class Functions {
        private static readonly byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static readonly byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        public static int pUserId { get; set; }
        public static string pUserLoginName { get; set; }
        public static string pUserFullName { get; set; }


        public static bool ValidaCpf(string cpf) {

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;

            int soma;
            int resto;

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11) {
                return false;
            }
            tempCpf = cpf.Substring(0, 9);

            soma = 0;

            for (int i = 0; i < 9; i++) {
                soma += int.Parse(tempCpf[i].ToString()) * (multiplicador1[i]);
            }
            resto = soma % 11;

            if (resto < 2) {
                resto = 0;
            } else {
                resto = 11 - resto;
            }

            digito = resto.ToString();
            tempCpf += digito;
            int soma2 = 0;

            for (int i = 0; i < 10; i++) {
                soma2 += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            }

            resto = soma2 % 11;

            if (resto < 2) {
                resto = 0;
            } else {
                resto = 11 - resto;
            }

            digito += resto.ToString();
            return cpf.EndsWith(digito);
        }

        public static bool ValidaCNPJ(string vrCNPJ) {
            string CNPJ = vrCNPJ.Replace(".", "");
            CNPJ = CNPJ.Replace("/", "");
            CNPJ = CNPJ.Replace("-", "");
            int[] digitos, soma, resultado;
            int nrDig;
            string ftmt;
            bool[] CNPJOk;

            ftmt = "6543298765432";
            digitos = new int[14];
            soma = new int[2];
            soma[0] = 0;
            soma[1] = 0;
            resultado = new int[2];
            resultado[0] = 0;
            resultado[1] = 0;
            CNPJOk = new bool[2];
            CNPJOk[0] = false;
            CNPJOk[1] = false;

            try {
                for (nrDig = 0; nrDig < 14; nrDig++) {
                    digitos[nrDig] = int.Parse(
                        CNPJ.Substring(nrDig, 1));
                    if (nrDig <= 11)
                        soma[0] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig + 1, 1)));

                    if (nrDig <= 12)

                        soma[1] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig, 1)));
                }

                for (nrDig = 0; nrDig < 2; nrDig++) {
                    resultado[nrDig] = (soma[nrDig] % 11);
                    if ((resultado[nrDig] == 0) || (
                         resultado[nrDig] == 1))
                        CNPJOk[nrDig] = (digitos[12 + nrDig] == 0);
                    else
                        CNPJOk[nrDig] = (digitos[12 + nrDig] == (11 - resultado[nrDig]));
                }

                return (CNPJOk[0] && CNPJOk[1]);
            } catch {
                return false;
            }
        }

        public static bool IsDate(object date) {
            try {
                DateTime dt = DateTime.Parse(date.ToString());
                return true;
            } catch {
                return false;
            }
        }

        public static string RetornaNumero(string Numero) {
            if (String.IsNullOrEmpty(Numero))
                return "0";
            else
                return Regex.Replace(Numero, @"[^\d]", "");
        }

        public static string FormatarCpfCnpj(string strCpfCnpj) {
            if (strCpfCnpj.Length <= 11) {
                MaskedTextProvider mtpCpf = new MaskedTextProvider(@"000\.000\.000-00");
                mtpCpf.Set(ZerosEsquerda(strCpfCnpj, 11));
                return mtpCpf.ToString();
            } else {
                MaskedTextProvider mtpCnpj = new MaskedTextProvider(@"00\.000\.000/0000-00");
                mtpCnpj.Set(ZerosEsquerda(strCpfCnpj, 11));
                return mtpCnpj.ToString();
            }
        }

        public static string ZerosEsquerda(string strString, int intTamanho) {
            string strResult = "";
            for (int intCont = 1; intCont <= (intTamanho - strString.Length); intCont++) {
                strResult += "0";
            }
            return strResult + strString;
        }

        public static string Encrypt(string clearText) {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = System.Text.Encoding.Unicode.GetBytes(clearText);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        public static string Decrypt(string cipherText) {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Convert.FromBase64String(cipherText);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return System.Text.Encoding.Unicode.GetString(outputBuffer);

        }

        public static int RetornaDvProcesso(int Numero) {
            int soma = 0, index = 0, Mult = 6;
            string sNumProc = Numero.ToString().PadLeft(5, '0');
            while (index < 5) {
                int nChar = Convert.ToInt32(sNumProc.Substring(index, 1));
                soma += (Mult * nChar);
                Mult--;
                index++;
            }

            int DigAux = soma % 11;
            int Digito = 11 - DigAux;
            if (Digito == 10)
                Digito = 0;
            if (Digito == 11)
                Digito = 1;

            return Digito;
        }

        public static int RetornaDVDocumento(int nNumDoc) {
            String sFromN ;
            Int32 nDV ;
            Int32 nTotPosAtual ;

            sFromN = nNumDoc.ToString("00000000");
            nTotPosAtual = Convert.ToInt32(sFromN.Substring(0, 1)) * 7;
            nTotPosAtual += Convert.ToInt32(sFromN.Substring(1, 1)) * 3;
            nTotPosAtual += Convert.ToInt32(sFromN.Substring(2, 1)) * 9;
            nTotPosAtual += Convert.ToInt32(sFromN.Substring(3, 1)) * 7;
            nTotPosAtual += Convert.ToInt32(sFromN.Substring(4, 1)) * 3;
            nTotPosAtual += Convert.ToInt32(sFromN.Substring(5, 1)) * 9;
            nTotPosAtual += Convert.ToInt32(sFromN.Substring(6, 1)) * 7;
            nTotPosAtual += Convert.ToInt32(sFromN.Substring(7, 1)) * 3;

            nDV = Convert.ToInt32(nTotPosAtual.ToString().Substring(nTotPosAtual.ToString().Length - 1));
            return nDV;
        }

        public static int Modulo11(string sValue) {
            int nDV;
            int nTotPosAtual ;

            nTotPosAtual = Convert.ToInt32(sValue.Substring(0, 1)) * 6;
            nTotPosAtual += Convert.ToInt32(sValue.Substring(1, 1)) * 5;
            nTotPosAtual += Convert.ToInt32(sValue.Substring(2, 1)) * 4;
            nTotPosAtual += Convert.ToInt32(sValue.Substring(3, 1)) * 3;
            nTotPosAtual += Convert.ToInt32(sValue.Substring(4, 1)) * 2;
            nTotPosAtual += Convert.ToInt32(sValue.Substring(5, 1)) * 9;
            nTotPosAtual += Convert.ToInt32(sValue.Substring(6, 1)) * 8;
            nTotPosAtual += Convert.ToInt32(sValue.Substring(7, 1)) * 7;
            nTotPosAtual += Convert.ToInt32(sValue.Substring(8, 1)) * 6;
            nTotPosAtual += Convert.ToInt32(sValue.Substring(9, 1)) * 5;
            nTotPosAtual += Convert.ToInt32(sValue.Substring(10, 1)) * 4;
            nTotPosAtual += Convert.ToInt32(sValue.Substring(11, 1)) * 3;
            nTotPosAtual += Convert.ToInt32(sValue.Substring(12, 1)) * 2;

            nDV = 11 - (nTotPosAtual % 11);

            if (nDV == 1)
                nDV = 0;
            else
                if (nDV == 10)
                nDV = 1;
            else
                    if (nDV == 1 | nDV == 11)
                nDV = 0;

            return nDV;
        }

        public static int RetornaDV2of5(string sBloco) {
            int nRet, d , nSoma = 0, nResto ;
            string e;

            for (int c = sBloco.Length - 1; c >= 0; c--) {
                if (c % 2 == 0)
                    d = Convert.ToInt16(sBloco.Substring(c, 1)) * 2;
                else {
                    d = Convert.ToInt16(sBloco.Substring(c, 1));
                }
                if (d > 0) {
                    if (d > 9) {
                        e = d.ToString();
                        d = Convert.ToInt32(e.Substring(0, 1)) + Convert.ToInt32(e.Substring(1, 1));
                    }
                    nSoma += d;
                }
            }
            nResto = nSoma % 10;
            if (nResto == 0)
                nRet = 0;
            else
                nRet = 10 - nResto;

            return nRet;
        }

        public static string Gera2of5Cod(string Valor, DateTime Vencimento, int Numero_Documento, int Codigo_Reduzido) {
            string sValor_Parcela = Convert.ToInt32(RetornaNumero(Valor)).ToString("00000000000");
            string sDia = Vencimento.Day.ToString("00");
            string sMes = Vencimento.Month.ToString("00");
            string sAno = Vencimento.Year.ToString();
            string sNumero_Documento = Numero_Documento.ToString("000000000");
            string sCodigo_Reduzido = Codigo_Reduzido.ToString("00000000");

            string sBloco1 = "816" + sValor_Parcela.Substring(0, 7);
            string sBloco2 = sValor_Parcela.Substring(7, 4) + "2177" + sAno.Substring(0, 3);
            string sBloco3 = sAno.Substring(3, 1) + sMes + sDia + sNumero_Documento.Substring(0, 6);
            string sBloco4 = sNumero_Documento.Substring(6, 3) + sCodigo_Reduzido;

            string sDv0 = RetornaDV2of5(sBloco1 + sBloco2 + sBloco3 + sBloco4).ToString();
            sBloco1 = sBloco1.Substring(0, 3) + sDv0 + sBloco1.Substring(3, 7);
            sBloco1 += "-" + RetornaDV2of5(sBloco1);
            sBloco2 += "-" + RetornaDV2of5(sBloco2);
            sBloco3 += "-" + RetornaDV2of5(sBloco3);
            sBloco4 += "-" + RetornaDV2of5(sBloco4);

            return sBloco1 + sBloco2 + sBloco3 + sBloco4;
        }

        public static string Gera2of5Str(string sCodigo_Barra) {
            string DataToEncode; string DataToPrint = ""; char StartCode = (char)203; char StopCode = (char)204; int CurrentChar ;
            DataToEncode = sCodigo_Barra;
            if (DataToEncode.Length % 2 == 1)
                DataToEncode = "0" + DataToEncode;
            for (int i = 0; i < DataToEncode.Length; i += 2) {
                CurrentChar = Convert.ToInt32(DataToEncode.Substring(i, 2));
                if (CurrentChar < 94)
                    DataToPrint += Convert.ToChar(CurrentChar + 33);
                else if (CurrentChar > 93)
                    DataToPrint += Convert.ToChar(CurrentChar + 103);
            }

            return StartCode + DataToPrint + StopCode;
        }

        public static int Calculo_DV10(string sValue) {
            Int32 nDV ;
            Int32 intNumero ;
            Int32 intTotalNumero = 0;
            Int32 intMultiplicador = 2;

            for (Int32 intContador = sValue.Length; intContador > 0; intContador--) {
                intNumero = Convert.ToInt32(sValue.Substring(intContador - 1, 1)) * intMultiplicador;
                if (intNumero > 9)
                    intNumero = Convert.ToInt32(intNumero.ToString().Substring(0, 1)) + Convert.ToInt32(intNumero.ToString().Substring(1, 1));

                intTotalNumero += intNumero;
                intMultiplicador = intMultiplicador == 2 ? 1 : 2;
            }

            Int32 DezenaSuperior = intTotalNumero < 10 ? 10 : (10 * (Convert.ToInt32(intTotalNumero.ToString().Substring(0, 1)) + 1));

            Int32 intResto = DezenaSuperior - intTotalNumero;

            if (intResto == 0 || intResto == 10)
                nDV = 0;
            else
                nDV = intResto;

            return nDV;
        }

        public static ProcessoNumero Split_Processo_Numero(string Numero) {
            int _dv = Convert.ToInt32( Numero.Substring(Numero.IndexOf("-") + 1, 1));
            int _numero = Convert.ToInt32( Numero.Substring(0, Numero.IndexOf("-")));
            int _ano = Convert.ToInt32( (Numero.Substring(Numero.Length - 4)));

            ProcessoNumero _reg = new ProcessoNumero {
                Ano=_ano,
                Numero=_numero,
                Dv=_dv
            };
            return _reg;
        }

        //public static IConfigurationRoot GetConfiguration() {
        //    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        //    return builder.Build();
        //}

        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();
        public static int GetRandomNumber() {
            lock (syncLock) { // synchronize
                return getrandom.Next(1, 2000000);
            }
        }

        public static modelCore.TipoCadastro Tipo_Cadastro(int Codigo) {
            modelCore.TipoCadastro _tipo_cadastro;
            if (Codigo < 100000)
                _tipo_cadastro = modelCore.TipoCadastro.Imovel;
            else {
                if (Codigo >= 100000 && Codigo < 500000)
                    _tipo_cadastro = modelCore.TipoCadastro.Empresa;
                else
                    _tipo_cadastro = modelCore.TipoCadastro.Cidadao;
            }
            return _tipo_cadastro;
        }



    }

}
