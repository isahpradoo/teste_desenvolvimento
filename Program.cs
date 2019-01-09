using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Prova_Desenvolvimento_Net
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = System.IO.File.ReadAllText(@"../../../Baseficticia.txt");

            var registers = text.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(1);

            var dt = new DataTable();
            dt.Columns.Add("NomeCliente", typeof(string));
            dt.Columns.Add("CEP", typeof(string));
            dt.Columns.Add("RuaComComplemento", typeof(string));
            dt.Columns.Add("Bairro", typeof(string));
            dt.Columns.Add("Cidade", typeof(string));
            dt.Columns.Add("Estado", typeof(string));
            dt.Columns.Add("ValorFatura", typeof(string));
            dt.Columns.Add("NumeroPaginas", typeof(int));


            foreach (var item in registers)
            {
                var columns = item.Split(";");

                Match cepNumeroRepetido = Regex.Match(columns[1], @"(\d)\1{7}", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Match cepValido = Regex.Match(columns[1], @"\d{8}\s+", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                if (!cepNumeroRepetido.Success && cepValido.Success)
                {
                    dt.Rows.Add(columns[0], columns[1], columns[2], columns[3], columns[4], columns[5], columns[6], columns[7]);
                }
            }

            var faturasAte6Pag = dt.AsEnumerable().Where(x => x.Field<int>("NumeroPaginas") <= 6).OrderBy( x=> x.Field<int>("NumeroPaginas")).CopyToDataTable();
            var faturasAte12Pag = dt.AsEnumerable().Where(x => x.Field<int>("NumeroPaginas") <= 12).OrderBy(x => x.Field<int>("NumeroPaginas")).CopyToDataTable();
            var faturasMais12Pag = dt.AsEnumerable().Where(x => x.Field<int>("NumeroPaginas") > 12).OrderBy(x => x.Field<int>("NumeroPaginas")).CopyToDataTable();
            var faturasZeradas = dt.AsEnumerable().Where(x => x.Field<string>("ValorFatura") == "0").CopyToDataTable();
            
            Helpers.ToCSV(faturasZeradas, "../../../faturasZeradas.csv");
            Helpers.ToCSV(faturasAte6Pag, "../../../faturasAtePag6.csv");
            Helpers.ToCSV(faturasAte12Pag, "../../../faturasAtePag12.csv");
            Helpers.ToCSV(faturasMais12Pag, "../../../faturasMais12Pag.csv");
        }
    }
}
