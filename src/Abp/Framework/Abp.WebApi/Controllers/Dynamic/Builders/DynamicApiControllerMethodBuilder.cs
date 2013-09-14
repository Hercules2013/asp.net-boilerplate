namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// Used to build <see cref="DynamicApiActionInfo"/> object.
    /// </summary>
    /// <typeparam name="T">Type of the proxied object</typeparam>
    internal class ApiControllerActionBuilder<T> : IApiControllerActionBuilder<T>
    {
        /// <summary>
        /// Reference to the <see cref="ApiControllerBuilder{T}"/> which created this object.
        /// </summary>
        private readonly ApiControllerBuilder<T> _controllerBuilder;

        /// <summary>
        /// The action which is being defined.
        /// </summary>
        private readonly DynamicApiActionInfo _methodInfo;

        /// <summary>
        /// Creates a new <see cref="ApiControllerActionBuilder{T}"/> object.
        /// </summary>
        /// <param name="apiControllerBuilder">Reference to the <see cref="ApiControllerBuilder{T}"/> which created this object</param>
        /// <param name="context">Reference to the current context</param>
        /// <param name="methodName">Name of the method which is being defined</param>
        public ApiControllerActionBuilder(ApiControllerBuilder<T> apiControllerBuilder, DynamicApiBuildContext context, string methodName)
        {
            _controllerBuilder = apiControllerBuilder;

            _methodInfo = new DynamicApiActionInfo(methodName, typeof(T).GetMethod(methodName));
            context.CustomizedMethods[methodName] = _methodInfo;
        }

        /// <summary>
        /// Used to specify Http verb of the action.
        /// </summary>
        /// <param name="verb">Http very</param>
        /// <returns>Action builder</returns>
        public IApiControllerActionBuilder<T> WithVerb(HttpVerb verb)
        {
            _methodInfo.Verb = verb;
            return this;
        }

        /// <summary>
        /// Used to specify name of the action.
        /// </summary>
        /// <param name="name">Action name</param>
        /// <returns></returns>
        public IApiControllerActionBuilder<T> WithActionName(string name)
        {
            _methodInfo.ActionName = name;
            return this;
        }

        /// <summary>
        /// Used to specify another method definition.
        /// </summary>
        /// <param name="methodName">Name of the method in proxied type</param>
        /// <returns>Action builder</returns>
        public IApiControllerActionBuilder<T> ForMethod(string methodName)
        {
            return _controllerBuilder.ForMethod(methodName);
        }

        /// <summary>
        /// Builds the controller.
        /// This method must be called at last of the build operation.
        /// </summary>
        public void Build()
        {
            _controllerBuilder.Build();
        }
    }
}