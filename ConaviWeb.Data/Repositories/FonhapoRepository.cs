using Dapper;
using ConaviWeb.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConaviWeb.Data.Repositories
{
    public class FonhapoRepository : IFonhapoRepository
    {
        private readonly MySQLConfiguration _connectionString;
        public FonhapoRepository(MySQLConfiguration connectionString)
        {
            _connectionString = connectionString;
        }

        protected MySqlConnection DbConnection()
        {
            return new MySqlConnection(_connectionString.ConnectionString);
        }
        public async Task<IEnumerable<string>> GetFonhapo()
        {
            var db = DbConnection();

            var sql = @"
                        SELECT 
    fonhapo.curp Curp
FROM proyecto_emergente.fonhapo
WHERE estatus = 1";

            return await db.QueryAsync<string>(sql, new { });
        }

        public async Task<bool> UpdateFonhapo(Fonhapo fonhapo)
        {
            var db = DbConnection();

            var sql = @"
                        UPDATE proyecto_emergente.fonhapo 
                        SET folio = @Folio,
monto_accion = @Monto_accion,
cve_modalidad = @Cve_modalidad,
fecha_movimiento = @Fecha_movimiento,
fecha_consulta = @Fecha_consulta,
paterno = @Paterno,
materno = @Materno,
nombre = @Nombre,
id_estado = @Id_estado,
des_estado = @Des_estado,
id_municipio = @Id_municipio,
des_municipio = @Des_municipio,
mensaje = @Mensaje,
estatus =2 
                        WHERE curp = @Curp";

            var result = await db.ExecuteAsync(sql, new
            {
                fonhapo.Folio,
                fonhapo.Monto_accion,
                fonhapo.Cve_modalidad,
                fonhapo.Fecha_movimiento,
                fonhapo.Fecha_consulta,
                fonhapo.Paterno,
                fonhapo.Materno,
                fonhapo.Nombre,
                fonhapo.Id_estado,
                fonhapo.Des_estado,
                fonhapo.Id_municipio,
                fonhapo.Des_municipio,
                fonhapo.Mensaje,
                fonhapo.Curp,
            });
            return result > 0;
        }
    }
}
