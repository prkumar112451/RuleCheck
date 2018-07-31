using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleBuilder
{
    public class MainWindowsViewModel :ViewModelBase
    {
        private ObservableCollection<string> templates = new ObservableCollection<string>();
        private ObservableCollection<string> rules = new ObservableCollection<string>();
        private ObservableCollection<SignalDetail> signals = new ObservableCollection<SignalDetail>();

        private string selectedTemplate;
        private string selectedRule;
        private string inputRecord;

        public MainWindowsViewModel()
        {
            TemplatePath = ConfigurationManager.AppSettings.Get("templatePath");
            RulePath = ConfigurationManager.AppSettings.Get("rulePath");

            LoadTemplates(TemplatePath);
            LoadRuless();
        }

        //public string FileName
        //{
        //    get { return fileName; }
        //    set
        //    {
        //        this.Set<string>(() => FileName, ref fileName, value);
        //        if (string.IsNullOrEmpty(fileName))
        //            InputRecord = null;
        //    }
        //}

        public string RulePath { get; set; }

        public string TemplatePath { get; set; }

        public string InputRecord { get; set; }

        public string RuleRecord { get; set; }

        public ObservableCollection<string> Templates
        {
            get { return templates; }
            set { this.Set<ObservableCollection<string>>(() => Templates, ref templates, value); }
        }

        public ObservableCollection<string> Rules
        {
            get { return rules; }
            set { this.Set<ObservableCollection<string>>(() => Rules, ref rules, value); }
        }

        public ObservableCollection<SignalDetail> Signals
        {
            get { return signals; }
            set { this.Set<ObservableCollection<SignalDetail>>(() => Signals, ref signals, value); }
        }

        private void LoadTemplates(string appPath)
        {
            List<string> temp = new List<string>();

            try
            {
                string[] XMLfiles = Directory.GetFiles(appPath, "*.json");
                foreach (string file in XMLfiles)
                {
                    temp.Add(Path.GetFileName(file));
                }
            }
            catch
            {

            }
            Templates = new ObservableCollection<string>(temp);
        }


        private void LoadRuless()
        {
            List<string> temp = new List<string>();

            try
            {
                string[] XMLfiles = Directory.GetFiles(RulePath, "*.json");
                foreach (string file in XMLfiles)
                {
                    temp.Add(Path.GetFileName(file));
                }
            }
            catch
            {

            }
            Rules = new ObservableCollection<string>(temp);
        }

        public string SelectedTemplate
        {
            get { return selectedTemplate; }
            set
            {
                this.Set<string>(() => SelectedTemplate, ref selectedTemplate, value);
                LoadTemplateData();
            }
        }

        public string SelectedRule
        {
            get { return selectedRule; }
            set
            {
                this.Set<string>(() => SelectedRule, ref selectedRule, value);
                LoadRule();
            }
        }

        IDictionary<string, SignalDetail> dict = new Dictionary<string, SignalDetail>();
        public List<SignalDetail> getResult = new List<SignalDetail>();

        private void LoadTemplateData()
        {
            string fileName = TemplatePath + "\\" + selectedTemplate;
            using (StreamReader fileReader = new StreamReader(fileName))
            {
                InputRecord = fileReader.ReadToEnd();
            }

            getResult = JsonConvert.DeserializeObject<List<SignalDetail>>(InputRecord);


            foreach (var item in getResult)
            {
                dict.Add(item.Signal, item);
            }
        }

        private void LoadRule()
        {
            string rulePath = RulePath + "\\" + selectedRule;
            using (StreamReader fileReader = new StreamReader(rulePath))
            {
                RuleRecord = fileReader.ReadToEnd();
            }

            var items = JsonConvert.DeserializeObject<List<SignalDetail>>(RuleRecord);

            ProcessRule(items);                      
        }

        private void ProcessRule(List<SignalDetail> items)
        {
            foreach (var item in items)
            {
                var value = item.Rule.Split(' ');
                var last = value.LastOrDefault();

                if (string.Compare(dict[item.Signal].ValueType, "integer") == 0)
                {
                    var itemValue = Convert.ToInt32(dict[item.Signal].Value);
                    var ruleValue = Convert.ToInt32(last);

                    if ((item.Rule.Contains("greater than") && itemValue > ruleValue) ||
                        (item.Rule.Contains("less than") && itemValue < ruleValue) ||
                        item.Rule.Contains("equal to") && itemValue == ruleValue)
                    {
                        dict[item.Signal].IsTrue = true;
                    }
                }

                if (string.Compare(dict[item.Signal].ValueType, "string") == 0)
                {
                    var itemValue = dict[item.Signal].Value;
                    var ruleValue = last;

                    if (item.Rule.Contains("not be"))
                    {
                        if (!string.Equals(itemValue, ruleValue))
                            dict[item.Signal].IsTrue = true;
                    }
                    else
                    {
                        dict[item.Signal].IsTrue = true;
                    }
                }

                if (string.Compare(dict[item.Signal].ValueType, "Datetime") == 0)
                {
                    var itemValue = Convert.ToDateTime(dict[item.Signal].Value);
                    var now = DateTime.Now;
                    var result = DateTime.Compare(itemValue, now);

                    if (item.Rule.Contains("future") && result == 1 ||
                        item.Rule.Contains("past") && result == -1 ||
                        item.Rule.Contains("present") && result == 0)
                    {
                        dict[item.Signal].IsTrue = true;

                    }
                }

                foreach(var dictionaryValue in dict)
                {
                    if(dictionaryValue.Value.IsTrue)
                    Signals.Add(dictionaryValue.Value);
                }
            }
        }
    }
}
