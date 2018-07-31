using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleBuilder
{
    public class ViewModelLocator
    {
        private readonly StandardKernel kernel;
        private static ViewModelLocator locator;

        private ViewModelLocator()
        {
            this.kernel = new StandardKernel(new RunTimeModule());
        }

        public static ViewModelLocator GetInstance()
        {
            if (locator == null)
                locator = new ViewModelLocator();
            return locator;
        }

        public static object Get(string name)
        {
            return GetInstance().GetViewModel(name);
        }

        public object GetViewModel(string name)
        {
            return kernel.Get<object>(name);
        }

        public static T Get<T>()
        {
            return GetInstance().GetViewModel<T>();
        }

        public T GetViewModel<T>()
        {
            return kernel.Get<T>();
        }
    }

}
