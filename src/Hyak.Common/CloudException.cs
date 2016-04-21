using System;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace Sandboxable.Hyak.Common
{
    /// <summary>
    /// Exception thrown for any invalid response.
    /// </summary>
    public class CloudException : Exception
    {
        /// <summary>
        /// Gets the error returned from the server.
        /// </summary>
        public CloudError Error
        {
            get;
            set;
        }

        /// <summary>
        /// Gets information about the associated HTTP request.
        /// </summary>
        public CloudHttpRequestErrorInfo Request
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets information about the associated HTTP response.
        /// </summary>
        public CloudHttpResponseErrorInfo Response
        {
            get;
            protected set;
        }

        /// <summary>
        /// Initializes a new instance of the CloudException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public CloudException(string message) : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CloudException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public CloudException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Create a CloudException from a failed response.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="requestContent">The HTTP request content.</param>
        /// <param name="response">The HTTP response.</param>
        /// <param name="responseContent">The HTTP response content.</param>
        /// <param name="innerException">Optional inner exception.</param>
        /// <returns>A CloudException representing the failure.</returns>
        public static CloudException Create(HttpRequestMessage request, string requestContent, HttpResponseMessage response, string responseContent, Exception innerException = null)
        {
            string reasonPhrase;

            var cloudError = ParseXmlOrJsonError(responseContent);
            var code = cloudError.Code;
            var message = cloudError.Message;

            if (code != null && message != null)
            {
                reasonPhrase = string.Concat(code, ": ", message);
            }
            else if (message != null)
            {
                reasonPhrase = message;
            }
            else if (code != null)
            {
                reasonPhrase = code;
            }
            else if (!string.IsNullOrEmpty(responseContent))
            {
                reasonPhrase = responseContent;
            }
            else if (response?.ReasonPhrase == null)
            {
                reasonPhrase = response?.StatusCode.ToString() ?? new InvalidOperationException().Message;
            }
            else
            {
                reasonPhrase = response.ReasonPhrase;
            }

            var cloudException = new CloudException(reasonPhrase, innerException)
            {
                Error = cloudError,
                Request = CloudHttpRequestErrorInfo.Create(request, requestContent),
                Response = CloudHttpResponseErrorInfo.Create(response, responseContent)
            };

            return cloudException;
        }

        /// <summary>
        /// Returns first non whitespace character
        /// </summary>
        /// <param name="content">Text to search in</param>
        /// <returns>Non whitespace or default char</returns>
        private static char FirstNonWhitespaceCharacter(string content)
        {
            content = content?.Trim();

            if (string.IsNullOrEmpty(content))
            {
                return '\0';
            }

            return content[0];
        }

        /// <summary>
        /// Checks if content is possibly a JSON.
        /// </summary>
        /// <param name="content">String to check.</param>
        /// <param name="validate">If set to true will validate entire JSON for validity 
        /// otherwise will just check the first character.</param>
        /// <returns>True is content is possibly an JSON otherwise false.</returns>
        public static bool IsJson(string content, bool validate = false)
        {
            bool flag;

            var chr = FirstNonWhitespaceCharacter(content);

            if (!validate)
            {
                return chr == '{';
            }

            if (chr != '{')
            {
                return false;
            }

            try
            {
                JObject.Parse(content);
                flag = true;
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        /// <summary>
        /// Checks if content is possibly an XML.
        /// </summary>
        /// <param name="content">String to check.</param>
        /// <param name="validate">If set to true will validate entire XML for validity 
        /// otherwise will just check the first character.</param>
        /// <returns>True is content is possibly an XML otherwise false.</returns>
        public static bool IsXml(string content, bool validate = false)
        {
            bool flag;

            var chr = FirstNonWhitespaceCharacter(content);

            if (!validate)
            {
                return chr == '<';
            }

            if (chr != '<')
            {
                return false;
            }

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                XDocument.Parse(content);

                flag = true;
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        /// <summary>
        /// Parse the response content as an JSON error message.
        /// </summary>
        /// <param name="content">The response content.</param>
        /// <returns>
        /// An object containing the parsed error code and message.
        /// </returns>
        public static CloudError ParseJsonError(string content)
        {
            string code = null;
            string message = null;

            if (content != null)
            {
                try
                {
                    var jObject = JObject.Parse(content);
                    if (jObject.GetValue("error", StringComparison.CurrentCultureIgnoreCase) == null)
                    {
                        message = jObject.GetValue("message", StringComparison.CurrentCultureIgnoreCase).ToString();
                        code = jObject.GetValue("code", StringComparison.CurrentCultureIgnoreCase).ToString();
                    }
                    else
                    {
                        var value = jObject.GetValue("error", StringComparison.CurrentCultureIgnoreCase) as JObject;

                        // ReSharper disable once PossibleNullReferenceException
                        message = value.GetValue("message", StringComparison.CurrentCultureIgnoreCase).ToString();
                        code = value.GetValue("code", StringComparison.CurrentCultureIgnoreCase).ToString();
                    }
                }
                catch
                {
                    // ignored
                }
            }

            var cloudError = new CloudError
            {
                Code = code,
                Message = message,
                OriginalMessage = content
            };

            return cloudError;
        }

        /// <summary>
        /// Parse the response content as an XML error message.
        /// </summary>
        /// <param name="content">The response content.</param>
        /// <returns>
        /// An object containing the parsed error code and message.
        /// </returns>
        public static CloudError ParseXmlError(string content)
        {
            string value = null;
            string message = null;
            if (content != null)
            {
                try
                {
                    // ReSharper disable once PossibleNullReferenceException
                    foreach (var xElement in XDocument.Parse(content).Root.Elements())
                    {
                        if (!string.Equals("Code", xElement.Name.LocalName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (!string.Equals("Message", xElement.Name.LocalName, StringComparison.CurrentCultureIgnoreCase))
                            {
                                continue;
                            }

                            message = xElement.Value;
                        }
                        else
                        {
                            value = xElement.Value;
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            }

            var cloudError = new CloudError
            {
                Code = value,
                Message = message,
                OriginalMessage = content
            };

            return cloudError;
        }

        /// <summary>
        /// Parse the response content as either an XML or JSON error message.
        /// </summary>
        /// <param name="content">The response content.</param>
        /// <returns>
        /// An object containing the parsed error code and message.
        /// </returns>
        public static CloudError ParseXmlOrJsonError(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return new CloudError
                {
                    OriginalMessage = content
                };
            }

            if (IsJson(content))
            {
                return ParseJsonError(content);
            }

            if (IsXml(content))
            {
                return ParseXmlError(content);
            }

            return new CloudError
            {
                OriginalMessage = content
            };
        }
    }
}