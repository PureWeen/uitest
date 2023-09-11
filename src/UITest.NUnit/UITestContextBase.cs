﻿using UITest.Core;

namespace UITest.Appium.NUnit
{
    public abstract class UITestContextBase
    {
        static IUIClientContext? _uiTestContext;
        private IServerContext? _context;

        protected static IUIClientContext? UITestContext { get { return _uiTestContext; } }
        protected static TestDevice Device 
        {
            get 
            {
                return UITestContext == null
                    ? throw new InvalidOperationException($"Call {nameof(InitialSetup)} before accessing the {nameof(Device)} property.")
                    : UITestContext.Config.GetProperty<TestDevice>("TestDevice");
            }
        }

        protected static IApp App 
        {
            get 
            {
                return UITestContext == null
                    ? throw new InvalidOperationException($"Call {nameof(InitialSetup)} before accessing the {nameof(App)} property.")
                    : UITestContext.App;
            }
        }

        public abstract IConfig GetTestConfig();

        public void InitialSetup(IServerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            InitialSetup(context, false);
        }

        public void Reset()
        {
            if (_context == null)
            {
                throw new InvalidOperationException($"Cannot {nameof(Reset)} if {nameof(InitialSetup)} has not been called.");
            }

            InitialSetup(_context, true);
        }

        private void InitialSetup(IServerContext context, bool reset)
        {
            var testConfig = GetTestConfig();

            // Check to see if we have a context already from a previous test and re-use it as creating the driver is expensive
            if (reset || _uiTestContext == null)
            {
                _uiTestContext?.Dispose();
                _uiTestContext = context.CreateUIClientContext(testConfig);
            }

            if (_uiTestContext == null)
            {
                throw new InvalidOperationException("Failed to get the driver.");
            }
        }
    }
}