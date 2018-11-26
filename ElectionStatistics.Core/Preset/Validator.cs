using System;
using System.Linq;
using System.Collections.Generic;
using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Preset
{
    public class Validator
    {
        private ModelContext modelContext;
        private Parser parser;

        public Validator(ModelContext modelContext, Parser parser)
        {
            this.modelContext = modelContext;
            this.parser = parser;
        }

        public void Execute(string expression, ProtocolSet protocolSet)
        {
            CheckExpression(expression);
            CheckProtocolSet(protocolSet);

            Validate(expression, protocolSet.Id);
        }

        private void CheckExpression(string expression)
        {
            if (expression == null || expression == "")
            {
                throw new ValidationException("expression is empty string");
            }
        }

        private void CheckProtocolSet(ProtocolSet protocolSet)
        {
            if (protocolSet == null)
            {
                throw new ValidationException("protocolSet is not provided");
            }
        }

        private void Validate(string expression, int protocolSetId)
        {
            List<int> ids = parser.Execute(expression);

            ids.ForEach((id) =>
                {
                    var lineDescr = modelContext.Set<LineDescription>()
                        .Where(ld => ld.Id == id && ld.ProtocolSetId == protocolSetId)
                        .SingleOrDefault();
                    if (lineDescr == null)
                    {
                        throw new ValidationException($"LineDescription with Id: {id}, " +
                            $"ProtocolSetId: {protocolSetId} not found");
                    }
                });
        }
    }
}
