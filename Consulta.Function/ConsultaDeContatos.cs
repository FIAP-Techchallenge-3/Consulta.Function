using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using System.Data;
using Compartilhado;
using Dapper;

namespace Consulta.Function
{
	internal class ConsultaDeContatos
	{
		private readonly ConfiguracaoBanco _banco;

		public ConsultaDeContatos(IConfiguration configuracao)
		{
			var conexao = configuracao.GetConnectionString("SqlServer");
			_banco = new ConfiguracaoBanco(conexao);
		}

		[Function("ConsultaDeContatos")]
		public async Task<HttpResponseData> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "contatos")] HttpRequestData req)
		{
			using IDbConnection conexao = _banco.ObtenhaConexao();

			var contatos = await conexao.QueryAsync<Contato>(
				"SELECT Nome, Ddd, Telefone, Email FROM Contatos"
			);

			var resposta = req.CreateResponse(System.Net.HttpStatusCode.OK);
			await resposta.WriteAsJsonAsync(contatos);

			return resposta;
		}
	}
}
