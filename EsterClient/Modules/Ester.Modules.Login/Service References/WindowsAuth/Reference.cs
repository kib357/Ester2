﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.17626
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ester.Modules.Login.WindowsAuth {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WindowsAuth.IWindowsAuthService")]
    public interface IWindowsAuthService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWindowsAuthService/GetApiKey", ReplyAction="http://tempuri.org/IWindowsAuthService/GetApiKeyResponse")]
        string GetApiKey();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWindowsAuthService/GetApiKey", ReplyAction="http://tempuri.org/IWindowsAuthService/GetApiKeyResponse")]
        System.Threading.Tasks.Task<string> GetApiKeyAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IWindowsAuthServiceChannel : Ester.Modules.Login.WindowsAuth.IWindowsAuthService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WindowsAuthServiceClient : System.ServiceModel.ClientBase<Ester.Modules.Login.WindowsAuth.IWindowsAuthService>, Ester.Modules.Login.WindowsAuth.IWindowsAuthService {
        
        public WindowsAuthServiceClient() {
        }
        
        public WindowsAuthServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WindowsAuthServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WindowsAuthServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WindowsAuthServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string GetApiKey() {
            return base.Channel.GetApiKey();
        }
        
        public System.Threading.Tasks.Task<string> GetApiKeyAsync() {
            return base.Channel.GetApiKeyAsync();
        }
    }
}