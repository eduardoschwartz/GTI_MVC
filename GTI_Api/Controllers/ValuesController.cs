using GTI_Bll.Classes;
using GTI_Models.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GTI_Api.Controllers {
    public class ValuesController : ApiController {
        private readonly string _connection = "GTIconnection";

        
        public HttpResponseMessage Get(int id) {

            Imovel_bll imovelRepository = new Imovel_bll(_connection);

            bool _existe = imovelRepository.Existe_Imovel(id);
            if (!_existe)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Imóvel não cadastrado.");
            else {
                ImovelStruct _imovel = imovelRepository.Dados_Imovel(id);
                return Request.CreateResponse(HttpStatusCode.OK, _imovel);
            }

        }
        //// GET api/values
        //public IEnumerable<string> Get() {
        //    return new string[] { "value1", "value5" };
        //}

        //// GET api/values/5
        //public string Get(int id) {
        //    return "value";
        //}

        // POST api/values
        public void Post([FromBody] string value) {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value) {
        }

        // DELETE api/values/5
        public void Delete(int id) {
        }
    }
}
