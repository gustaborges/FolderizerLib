using System;
using System.Collections.Generic;

namespace FolderizerLib.Results
{
    public class OrganizationResult
    {
        private IList<Failure> _failures = new List<Failure>();
        
        public IList<Failure> Failures 
        {
            get => _failures;
        }

        public bool HasFailure 
        { 
            get => Failures.Count > 0; 
        }

        public void AppendFailure(string filePath, Exception ex)
        {
            if (filePath is null || ex is null)
            {
                throw new ArgumentNullException();
            }

            Failures.Add(new Failure(filePath, ex));
        }
    }

}