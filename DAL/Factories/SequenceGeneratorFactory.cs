using System;
using DAL.Interfaces;
using DAL.Model;

namespace DAL.Factories
{
    public class SequenceGeneratorFactory : ISequenceGeneratorFactory
    {
        public ISequenceGenerator GetSequence(string type)
        {
            if (type == DataDefinition.SequenceType.MaxImage)
                return new MaxImageSequenceGenerator();
            else
                throw new ArgumentException("CompositionFactory: Composition type argument not valid: {0}", type);
        }
    }
}
