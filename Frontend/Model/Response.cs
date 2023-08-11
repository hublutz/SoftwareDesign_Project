namespace Frontend.Model
{
    /// <summary>
    /// Enum <c>ResponseCode</c> represents constants indicating
    /// the required operation's success or failure
    /// </summary>
    public enum ResponseCode
    {
        OperationSucceededCode = 200,
        OperationFailedCode = 500
    }

    /// <summary>
    /// Class <c>Response</c> represents an operation's response from the Backend
    /// </summary>
    public class Response
    {
        /// <value>
        /// Member <c>code</c> holds the response code - indicating its
        /// success or failure
        /// </value>
        private ResponseCode code;
        public ResponseCode Code { get => code; set => code = value; }
        /// <summary>
        /// Member <c>message</c> is the response's returned data, if succeeded.
        /// Else, it holds the error message
        /// </summary>
        private object? message;
        public object? Message { get => message; set => message = value; }

        /// <summary>
        /// Initialises a response with a failure code and <c>null</c> message
        /// This constructor is used for Json deserialization
        /// </summary>
        public Response()
        {
            this.Code = ResponseCode.OperationFailedCode;
            this.Message = null;
        }

        /// <summary>
        /// Inititialises a new Response with code and message given
        /// </summary>
        /// <param name="code">The Response's code</param>
        /// <param name="message">The Response's returned data or error message. 
        /// null is default for empty response</param>
        public Response(ResponseCode code, object? message=null)
        {
            this.Code = code;
            this.Message = message;
        }
    }
}
