using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HARbinger.App.Models
{
    public class RootObject
    {
        public Log log { get; set; }
    }

    public class Log
    {
        public string version { get; set; }
        public Creator creator { get; set; }
        public object[] pages { get; set; }
        public Entries[] entries { get; set; }
    }

    public class Creator
    {
        public string name { get; set; }
        public string version { get; set; }
    }

    public class Entries
    {
        public string _fromCache { get; set; }
        public _initiator _initiator { get; set; }
        public string _priority { get; set; }
        public string _resourceType { get; set; }
        public Cache cache { get; set; }
        public Request request { get; set; }
        public Response response { get; set; }
        public string serverIPAddress { get; set; }
        public string startedDateTime { get; set; }
        public double time { get; set; }
        public Timings timings { get; set; }
        public string connection { get; set; }
    }

    public class _initiator
    {
        public string type { get; set; }
        public Stack stack { get; set; }
    }

    public class Stack
    {
        public CallFrames[] callFrames { get; set; }
        public Parent parent { get; set; }
    }

    public class CallFrames
    {
        public string functionName { get; set; }
        public string scriptId { get; set; }
        public string url { get; set; }
        public int lineNumber { get; set; }
        public int columnNumber { get; set; }
    }

    public class Parent
    {
        public string description { get; set; }
        public CallFrames1[] callFrames { get; set; }
        public Parent1 parent { get; set; }
    }

    public class CallFrames1
    {
        public string functionName { get; set; }
        public string scriptId { get; set; }
        public string url { get; set; }
        public int lineNumber { get; set; }
        public int columnNumber { get; set; }
    }

    public class Parent1
    {
        public string description { get; set; }
        public CallFrames2[] callFrames { get; set; }
        public Parent2 parent { get; set; }
    }

    public class CallFrames2
    {
        public string functionName { get; set; }
        public string scriptId { get; set; }
        public string url { get; set; }
        public int lineNumber { get; set; }
        public int columnNumber { get; set; }
    }

    public class Parent2
    {
        public string description { get; set; }
        public CallFrames3[] callFrames { get; set; }
        public Parent3 parent { get; set; }
    }

    public class CallFrames3
    {
        public string functionName { get; set; }
        public string scriptId { get; set; }
        public string url { get; set; }
        public int lineNumber { get; set; }
        public int columnNumber { get; set; }
    }

    public class Parent3
    {
        public string description { get; set; }
        public CallFrames4[] callFrames { get; set; }
        public Parent4 parent { get; set; }
    }

    public class CallFrames4
    {
        public string functionName { get; set; }
        public string scriptId { get; set; }
        public string url { get; set; }
        public int lineNumber { get; set; }
        public int columnNumber { get; set; }
    }

    public class Parent4
    {
        public string description { get; set; }
        public CallFrames5[] callFrames { get; set; }
    }

    public class CallFrames5
    {
        public string functionName { get; set; }
        public string scriptId { get; set; }
        public string url { get; set; }
        public int lineNumber { get; set; }
        public int columnNumber { get; set; }
    }

    public class Cache
    {

    }

    public class Request
    {
        public string method { get; set; }
        public string url { get; set; }
        public string httpVersion { get; set; }
        public Headers[] headers { get; set; }
        public QueryString[] queryString { get; set; }
        public Cookies[] cookies { get; set; }
        public int headersSize { get; set; }
        public int bodySize { get; set; }
        public PostData postData { get; set; }
    }

    public class Headers
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class QueryString
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Cookies
    {
        public string name { get; set; }
        public string value { get; set; }
        public string path { get; set; }
        public string domain { get; set; }
        public string expires { get; set; }
        public bool httpOnly { get; set; }
        public bool secure { get; set; }
        public string sameSite { get; set; }
    }

    public class PostData
    {
        public string mimeType { get; set; }
        public string text { get; set; }
    }

    public class Response
    {
        public int status { get; set; }
        public string statusText { get; set; }
        public string httpVersion { get; set; }
        public Headers1[] headers { get; set; }
        public object[] cookies { get; set; }
        public Content content { get; set; }
        public string redirectURL { get; set; }
        public int headersSize { get; set; }
        public int bodySize { get; set; }
        public int _transferSize { get; set; }
        public object _error { get; set; }
        public string _fulfilledBy { get; set; }
    }

    public class Headers1
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Content
    {
        public int size { get; set; }
        public string mimeType { get; set; }
        public string text { get; set; }
        public string encoding { get; set; }
        public int compression { get; set; }
    }

    public class Timings
    {
        public double blocked { get; set; }
        public double dns { get; set; }
        public double ssl { get; set; }
        public double connect { get; set; }
        public double send { get; set; }
        public double wait { get; set; }
        public double receive { get; set; }
        public double _blocked_queueing { get; set; }
    }


}
