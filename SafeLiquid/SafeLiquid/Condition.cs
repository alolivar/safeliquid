
// Type: SafeLiquid.Condition




using SafeLiquid.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SafeLiquid
{
    public class Condition
    {
        public static readonly Dictionary<string, ConditionOperatorDelegate> Operators = new Dictionary<string, ConditionOperatorDelegate>()
    {
      {
        "==",
        (ConditionOperatorDelegate) ((left, right) => Condition.EqualVariables(left, right))
      },
      {
        "!=",
        (ConditionOperatorDelegate) ((left, right) => !Condition.EqualVariables(left, right))
      },
      {
        "<>",
        (ConditionOperatorDelegate) ((left, right) => !Condition.EqualVariables(left, right))
      },
      {
        "<",
        (ConditionOperatorDelegate) ((left, right) => left != null && right != null && Comparer<object>.Default.Compare(left, Convert.ChangeType(right, left.GetType())) == -1)
      },
      {
        ">",
        (ConditionOperatorDelegate) ((left, right) => left != null && right != null && Comparer<object>.Default.Compare(left, Convert.ChangeType(right, left.GetType())) == 1)
      },
      {
        "<=",
        (ConditionOperatorDelegate) ((left, right) => left != null && right != null && Comparer<object>.Default.Compare(left, Convert.ChangeType(right, left.GetType())) <= 0)
      },
      {
        ">=",
        (ConditionOperatorDelegate) ((left, right) => left != null && right != null && Comparer<object>.Default.Compare(left, Convert.ChangeType(right, left.GetType())) >= 0)
      },
      {
        "contains",
        (ConditionOperatorDelegate) ((left, right) =>
        {
          switch (left)
          {
            case string _:
              return ((string) left).Contains((string) right);
            case IEnumerable _:
              return Condition.Any((IEnumerable) left, (Func<object, bool>) (element => element.BackCompatSafeTypeInsensitiveEqual(right)));
            default:
              return false;
          }
        })
      },
      {
        "startsWith",
        (ConditionOperatorDelegate) ((left, right) =>
        {
          switch (left)
          {
            case IList _:
              return Condition.EqualVariables(((IEnumerable) left).OfType<object>().FirstOrDefault<object>(), right);
            case string _:
              return ((string) left).StartsWith((string) right);
            default:
              return false;
          }
        })
      },
      {
        "endsWith",
        (ConditionOperatorDelegate) ((left, right) =>
        {
          switch (left)
          {
            case IList _:
              return Condition.EqualVariables(((IEnumerable) left).OfType<object>().LastOrDefault<object>(), right);
            case string _:
              return ((string) left).EndsWith((string) right);
            default:
              return false;
          }
        })
      },
      {
        "hasKey",
        (ConditionOperatorDelegate) ((left, right) => left is IDictionary && ((IDictionary) left).Contains(right))
      },
      {
        "hasValue",
        (ConditionOperatorDelegate) ((left, right) => left is IDictionary && ((IDictionary) left).Values.Cast<object>().Contains<object>(right))
      }
    };
        private string _childRelation;
        private Condition _childCondition;

        private static bool Any(IEnumerable enumerable, Func<object, bool> condition)
        {
            foreach (object obj in enumerable)
            {
                if (condition(obj))
                    return true;
            }
            return false;
        }

        public string Left { get; set; }

        public string Operator { get; set; }

        public string Right { get; set; }

        public List<object> Attachment { get; private set; }

        public virtual bool IsElse => false;

        public Template Template { get; set; }

        public Condition(Template template, string left, string @operator, string right)
            : this(template)
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }

        public Condition(Template template)
        {
            this.Template = template;
        }

        public virtual bool Evaluate(Context context, IFormatProvider formatProvider)
        {
            context = context ?? new Context(Template, formatProvider);

            if (context.Strainer == null) context.Strainer = Template.Strainer;

            bool flag = Condition.InterpretCondition(this.Left, this.Right, this.Operator, context);
            string childRelation = this._childRelation;
            if (!(childRelation == "or"))
            {
                if (!(childRelation == "and"))
                    return flag;
                return flag && this._childCondition.Evaluate(context, formatProvider);
            }
            return flag || this._childCondition.Evaluate(context, formatProvider);
        }

        public void Or(Condition condition)
        {
            this._childRelation = "or";
            this._childCondition = condition;
        }

        public void And(Condition condition)
        {
            this._childRelation = "and";
            this._childCondition = condition;
        }

        public List<object> Attach(List<object> attachment)
        {
            this.Attachment = attachment;
            return attachment;
        }

        public override string ToString() => string.Format("<Condition {0} {1} {2}>", (object)this.Left, (object)this.Operator, (object)this.Right);

        private static bool EqualVariables(object left, object right)
        {
            if (left is Symbol symbol1)
                return symbol1.EvaluationFunction(right);
            return right is Symbol symbol2 ? symbol2.EvaluationFunction(left) : left.SafeTypeInsensitiveEqual(right);
        }

        private static bool InterpretCondition(string left, string right, string op, Context context)
        {
            if (string.IsNullOrEmpty(op))
            {
                object obj = context[left, false];
                if (obj == null)
                    return false;
                return !(obj is bool flag2) || flag2;
            }
            object left1 = context[left];
            object right1 = context[right];
            string key = Condition.Operators.Keys.FirstOrDefault<string>((Func<string, bool>)(opk => opk.Equals(op) || opk.ToLowerInvariant().Equals(op) || Template.NamingConvention.OperatorEquals(opk, op)));
            if (key == null)
            {
                throw new SafeLiquid.Exceptions.ArgumentException(Liquid.ResourceManager.GetString("ConditionUnknownOperatorException"), new string[1]
        {
          op
        });
            }

            return Condition.Operators[key](left1, right1);
        }
    }
}
