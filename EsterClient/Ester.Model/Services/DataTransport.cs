using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Ester.Model.Interfaces;
using EsterCommon.Exceptions;
using Newtonsoft.Json;
using RestSharp;
using DataFormat = RestSharp.DataFormat;

namespace Ester.Model.Services
{

	//�����, ����� ������� ���� ������ � ��������

	public class DataTransport : IDataTransport
	{
        private readonly IServerInfo _serverInfo;
        private readonly ISessionInfo _sessionInfo;

		private static readonly List<HttpStatusCode> ValidHttpStatuses = new List<HttpStatusCode>()
        {
            HttpStatusCode.OK,
            HttpStatusCode.Created,
            HttpStatusCode.NoContent
        };

		static T DeSerializeToJson<T>(Stream stream)
		{
			//StreamReader reader = new StreamReader(stream);
			//string text = reader.ReadToEnd();

			using (stream)
			{
				//return JsonConvert.DeserializeObject<T>(text);
				var deserializer = new DataContractJsonSerializer(typeof(T));
				return (T)deserializer.ReadObject(stream);
			}
		}

		public DataTransport(IServerInfo serverInfo, ISessionInfo sessionInfo)
		{
			_sessionInfo = sessionInfo;
			_serverInfo = serverInfo;
		}

		private string CreateUri(string relativeUri, bool withApiKey)
		{
			string uri;
			if (withApiKey)
			{
				if (relativeUri.Contains("?"))
					uri = relativeUri + "&apikey=" + _sessionInfo.ApiKey;
				else
				{
					uri = relativeUri + "?apikey=" + _sessionInfo.ApiKey;
				}
			}
			else uri = relativeUri;

			return uri;
		}

		public async Task<T> GetRequestAsync<T>(string relativeUri, bool withApiKey = false, int timeout = 100000)
		{
			try
			{
				return await Task.Run(() => GetRequest<T>(relativeUri, withApiKey, timeout));
			}
			catch (Exception)
			{
				throw;
			}
		}

		public T GetRequest<T>(string relativeUri, bool withApiKey = false, int timeout = 100000, params JsonConverter[] jsonConverters)
		{
			try
			{
				var client = new RestClient { Timeout = 100000, BaseUrl = _serverInfo.CommonServerAddress };

				var uri = CreateUri(relativeUri, withApiKey);

				var request = new RestRequest(uri)
					{
						Method = Method.GET,
						Timeout = timeout
					};

				var response = client.Execute(request);

				if (response.ErrorException != null)
					throw response.ErrorException;

				if (!ValidHttpStatuses.Contains(response.StatusCode))
				{
					if (response.StatusCode == HttpStatusCode.BadRequest)
						throw new BadRequestException(response.StatusDescription);
					throw new Exception(response.StatusDescription);
				}
				return JsonConvert.DeserializeObject<T>(response.Content, jsonConverters);
			}
			catch (BadRequestException ex)
			{
				throw ex;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public T PostRequest<T>(dynamic data, string relativeUri, bool withApiKey = false, int timeout = 100000)
		{
			try
			{
				var client = new RestClient { Timeout = 100000, BaseUrl = _serverInfo.CommonServerAddress };

				var uri = CreateUri(relativeUri, withApiKey);

				var request = new RestRequest(uri)
					{
						Method = Method.POST,
						Timeout = timeout,
						RequestFormat = DataFormat.Json,
						JsonSerializer = new JsonConvertWrapper()
					};

				if (data != null)
					request.AddBody(data);

				var response = client.Execute(request);
				if (response.ErrorException != null) throw response.ErrorException;

				if (!ValidHttpStatuses.Contains(response.StatusCode))
				{
					if (response.StatusCode == HttpStatusCode.BadRequest)
						throw new BadRequestException(response.StatusDescription);
					throw new Exception(response.StatusDescription);
				}

				return response.Content == null
					? default(T)
					: JsonConvert.DeserializeObject<T>(response.Content);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

        public async Task<T> PostRequestAsync<T>(dynamic data, string relativeUri, bool withApiKey = false, int timeout = 100000)
		{
			try
			{
				return await Task.Run(() => PostRequest<T>(data, relativeUri, withApiKey, timeout));
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void DeleteRequest(string relativeUri, bool withApiKey = false, int timeout = 100000)
		{
			try
			{
				var client = new RestClient { Timeout = 100000, BaseUrl = _serverInfo.CommonServerAddress };
				var uri = CreateUri(relativeUri, withApiKey);

				var request = new RestRequest(uri)
				{
					Method = Method.DELETE,
					Timeout = timeout
				};

				var response = client.Execute(request);
				if (response.ErrorException != null) throw response.ErrorException;

				if (!ValidHttpStatuses.Contains(response.StatusCode))
				{
					if (response.StatusCode == HttpStatusCode.BadRequest)
						throw new BadRequestException(response.StatusDescription);
					throw new Exception(response.StatusDescription);
				}
			}
			catch (Exception)
			{
				throw;
			}

		}

		public async Task DeleteRequestAsync(string relativeUri, bool withApiKey = false, int timeout = 100000)
		{
			try
			{
				await Task.Run(() => DeleteRequest(relativeUri, withApiKey, timeout));
			}
			catch (Exception)
			{

				throw;
			}
		}
	}
}
