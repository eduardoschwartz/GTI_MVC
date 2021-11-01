

namespace GTI_Mvc.ViewModels {
    public class LancTribViewModel {
        public int Lancamento_Codigo { get; set; }
        public string Lancamento_Nome_Completo { get; set; }
        public string Lancamento_Nome_Reduzido { get; set; }
        public int Tributo_Codigo { get; set; }
        public string Tributo_Nome { get; set; }
        public string Artigo { get; set; }
        public decimal Valor { get; set; }
        public int Ano { get; set; }
    }
}