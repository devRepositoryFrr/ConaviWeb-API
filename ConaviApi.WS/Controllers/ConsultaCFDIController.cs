using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ConaviApi.WS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultaCFDIController : ControllerBase
    {
        public class CFDI
        {
            public string RFCEmisor { get; set; }
            public string RFCReceptor { get; set; }
            public string Total { get; set; }
            public string UUID { get; set; }
            public string Sello { get; set; }

        }

        [HttpGet]
        public async Task<IActionResult> ConsultaCFDIAsync([FromQuery] CFDI request)
        {
            try
            {

                var querySend = "?re=" + request.RFCEmisor + "&rr=" + request.RFCReceptor + "&tt=" + request.Total + "&id=" + request.UUID;

                ConsultaCFDI.ConsultaCFDIServiceClient oConsulta = new ConsultaCFDI.ConsultaCFDIServiceClient();
                ConsultaCFDI.Acuse oAcuse = new ConsultaCFDI.Acuse();

                oAcuse = await oConsulta.ConsultaAsync(querySend);

                oConsulta.Close();
                return Ok(oAcuse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return BadRequest();
        }

        
    }
}
