﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleApplication1.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.ServiceSoap")]
    public interface ServiceSoap {
        
        // CODEGEN: Generating message contract since element name str from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SayLouder", ReplyAction="*")]
        ConsoleApplication1.ServiceReference1.SayLouderResponse SayLouder(ConsoleApplication1.ServiceReference1.SayLouderRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SayLouder", ReplyAction="*")]
        System.Threading.Tasks.Task<ConsoleApplication1.ServiceReference1.SayLouderResponse> SayLouderAsync(ConsoleApplication1.ServiceReference1.SayLouderRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SayLouderRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SayLouder", Namespace="http://tempuri.org/", Order=0)]
        public ConsoleApplication1.ServiceReference1.SayLouderRequestBody Body;
        
        public SayLouderRequest() {
        }
        
        public SayLouderRequest(ConsoleApplication1.ServiceReference1.SayLouderRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class SayLouderRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string str;
        
        public SayLouderRequestBody() {
        }
        
        public SayLouderRequestBody(string str) {
            this.str = str;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SayLouderResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SayLouderResponse", Namespace="http://tempuri.org/", Order=0)]
        public ConsoleApplication1.ServiceReference1.SayLouderResponseBody Body;
        
        public SayLouderResponse() {
        }
        
        public SayLouderResponse(ConsoleApplication1.ServiceReference1.SayLouderResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class SayLouderResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string SayLouderResult;
        
        public SayLouderResponseBody() {
        }
        
        public SayLouderResponseBody(string SayLouderResult) {
            this.SayLouderResult = SayLouderResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ServiceSoapChannel : ConsoleApplication1.ServiceReference1.ServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceSoapClient : System.ServiceModel.ClientBase<ConsoleApplication1.ServiceReference1.ServiceSoap>, ConsoleApplication1.ServiceReference1.ServiceSoap {
        
        public ServiceSoapClient() {
        }
        
        public ServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ConsoleApplication1.ServiceReference1.SayLouderResponse ConsoleApplication1.ServiceReference1.ServiceSoap.SayLouder(ConsoleApplication1.ServiceReference1.SayLouderRequest request) {
            return base.Channel.SayLouder(request);
        }
        
        public string SayLouder(string str) {
            ConsoleApplication1.ServiceReference1.SayLouderRequest inValue = new ConsoleApplication1.ServiceReference1.SayLouderRequest();
            inValue.Body = new ConsoleApplication1.ServiceReference1.SayLouderRequestBody();
            inValue.Body.str = str;
            ConsoleApplication1.ServiceReference1.SayLouderResponse retVal = ((ConsoleApplication1.ServiceReference1.ServiceSoap)(this)).SayLouder(inValue);
            return retVal.Body.SayLouderResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ConsoleApplication1.ServiceReference1.SayLouderResponse> ConsoleApplication1.ServiceReference1.ServiceSoap.SayLouderAsync(ConsoleApplication1.ServiceReference1.SayLouderRequest request) {
            return base.Channel.SayLouderAsync(request);
        }
        
        public System.Threading.Tasks.Task<ConsoleApplication1.ServiceReference1.SayLouderResponse> SayLouderAsync(string str) {
            ConsoleApplication1.ServiceReference1.SayLouderRequest inValue = new ConsoleApplication1.ServiceReference1.SayLouderRequest();
            inValue.Body = new ConsoleApplication1.ServiceReference1.SayLouderRequestBody();
            inValue.Body.str = str;
            return ((ConsoleApplication1.ServiceReference1.ServiceSoap)(this)).SayLouderAsync(inValue);
        }
    }
}
