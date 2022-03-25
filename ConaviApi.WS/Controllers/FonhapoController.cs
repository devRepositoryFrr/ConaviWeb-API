using ConaviWeb.Data.Repositories;
using ConaviWeb.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConaviApi.WS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FonhapoController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IFonhapoRepository _fonhapoRepository;
        public FonhapoController(IWebHostEnvironment environment, IFonhapoRepository fonhapoRepository)
        {
            _environment = environment;
            _fonhapoRepository = fonhapoRepository;
        }
        public class RequestFonhapo
        {
            public string IpSol { get; set; }
            public string curp { get; set; }
        }
        [XmlRoot(ElementName = "getFhpoPadronResponse", Namespace = "http://fhpo.ws/")]
        public class ResponseFonhapo
        {
            [XmlElement("return")]
            public Item Return { get; set; }
        }
        public class Item
        {

            [XmlElement("folio")]
            public string Folio { get; set; }
            [XmlElement("monto_accion")]
            public string Monto_accion { get; set; }
            [XmlElement("cve_modalidad")]
            public string Cve_modalidad { get; set; }
            [XmlElement("fecha_movimiento")]
            public string Fecha_movimiento { get; set; }
            [XmlElement("fecha_consulta")]
            public string Fecha_consulta { get; set; }
            [XmlElement("paterno")]
            public string Paterno { get; set; }
            [XmlElement("materno")]
            public string Materno { get; set; }
            [XmlElement("nombre")]
            public string Nombre { get; set; }
            [XmlElement("curp")]
            public string Curp { get; set; }
            [XmlElement("id_estado")]
            public string Id_estado { get; set; }
            [XmlElement("des_estado")]
            public string Des_estado { get; set; }
            [XmlElement("id_municipio")]
            public string Id_municipio { get; set; }
            [XmlElement("des_municipio")]
            public string Des_municipio { get; set; }
            [XmlElement("mensaje")]
            public string Mensaje { get; set; }
        }

        [HttpGet]
        //public async Task<IActionResult> FonhapoAsync([FromForm]string curpsA)
        public async Task<IActionResult> FonhapoAsync()
        {
            List<string> curps = (List<string>)await _fonhapoRepository.GetFonhapo();
            //string[] curps = curpsA.Split(',');
            //var curps = new string[] {

            //};
            List<ResponseFonhapo> datos = new List<ResponseFonhapo>();
            Fonhapo fonhapo = new();
            foreach (var curp in curps)
            {

                RequestFonhapo request = new();
                request.IpSol = "200.78.235.215";
                request.curp = curp;

                var json = JsonConvert.SerializeObject(request);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var url = "https://www.fonhapo.gob.mx/ConsultaPadron/service/post";
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(10);

                var response = await client.PostAsync(url, data);

                var result = await response.Content.ReadAsStringAsync();
                var s = result;
                var buffer = Encoding.UTF8.GetBytes(s);
                MemoryStream stream = new MemoryStream(buffer);
                var serializer = new XmlSerializer(typeof(ResponseFonhapo));
                var responseFonhapo = (ResponseFonhapo)serializer.Deserialize(stream);

                //GetCSV(responseFonhapo);
                // return Ok(responseFonhapo);
                //then do whatever you want
                fonhapo.Folio = responseFonhapo.Return.Folio;
                fonhapo.Monto_accion = responseFonhapo.Return.Monto_accion;
                fonhapo.Cve_modalidad = responseFonhapo.Return.Cve_modalidad;
                fonhapo.Fecha_movimiento = responseFonhapo.Return.Fecha_movimiento;
                fonhapo.Fecha_consulta = responseFonhapo.Return.Fecha_consulta;
                fonhapo.Paterno = responseFonhapo.Return.Paterno;
                fonhapo.Materno = responseFonhapo.Return.Materno;
                fonhapo.Nombre = responseFonhapo.Return.Nombre;
                fonhapo.Id_estado = responseFonhapo.Return.Id_estado;
                fonhapo.Des_estado = responseFonhapo.Return.Des_estado;
                fonhapo.Id_municipio = responseFonhapo.Return.Id_municipio;
                fonhapo.Des_municipio = responseFonhapo.Return.Des_municipio;
                fonhapo.Mensaje = responseFonhapo.Return.Mensaje;
                fonhapo.Curp = responseFonhapo.Return.Curp;
                var success = await _fonhapoRepository.UpdateFonhapo(fonhapo);
                //datos.Add(responseFonhapo);

            }
            //var path = System.IO.Path.Combine(_environment.WebRootPath, "doc", "reporteFonhapo.csv");
            //FileContentResult file = SaveToCsv(datos);
            //return file;
            return Ok();
        }
        private FileContentResult SaveToCsv(List<ResponseFonhapo> reportData)
        {
            var items = reportData;

            bool isFirstIteration = true;
            StringBuilder sb = new StringBuilder();
            foreach (var item in items)
            {
                string[] propertyNames = item.Return.GetType().GetProperties().Select(p => p.Name).ToArray();
                foreach (var prop in propertyNames)
                {
                    if (isFirstIteration == true)
                    {
                        for (int j = 0; j < propertyNames.Length; j++)
                        {
                            sb.Append("\"" + propertyNames[j] + "\"" + ',');
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append("\r\n");
                        isFirstIteration = false;
                    }
                    object propValue = item.Return.GetType().GetProperty(prop).GetValue(item.Return, null);
                    sb.Append("\"" + propValue + "\"" + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("\r\n");
            }
            //return sb.ToString();
            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "Reporte_Fonhapo.csv");

        }
    }
}
