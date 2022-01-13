
// Type: SafeLiquid.Util.Symbol




using System;

namespace SafeLiquid.Util
{
  internal class Symbol
  {
    public Func<object, bool> EvaluationFunction { get; set; }

    public Symbol(Func<object, bool> evaluationFunction) => this.EvaluationFunction = evaluationFunction;
  }
}
