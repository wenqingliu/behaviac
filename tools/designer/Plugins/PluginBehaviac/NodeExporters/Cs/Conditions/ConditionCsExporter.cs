/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Behaviac.Design;
using Behaviac.Design.Nodes;
using PluginBehaviac.DataExporters;

namespace PluginBehaviac.NodeExporters
{
    public class ConditionCsExporter : NodeCsExporter
    {
        protected override bool ShouldGenerateClass(Node node)
        {
            PluginBehaviac.Nodes.Condition condition = node as PluginBehaviac.Nodes.Condition;
            return (condition != null);
        }

        protected override void GenerateConstructor(Node node, StreamWriter stream, string indent, string className)
        {
            base.GenerateConstructor(node, stream, indent, className);

            PluginBehaviac.Nodes.Condition condition = node as PluginBehaviac.Nodes.Condition;
            if (condition == null)
                return;

            if (condition.Opl != null)
            {
                RightValueCsExporter.GenerateClassConstructor(condition.Opl, stream, indent, "opl");
            }

            if (condition.Opr != null)
            {
                RightValueCsExporter.GenerateClassConstructor(condition.Opr, stream, indent, "opr");
            }
        }

        protected override void GenerateMember(Node node, StreamWriter stream, string indent)
        {
            base.GenerateMember(node, stream, indent);

            PluginBehaviac.Nodes.Condition condition = node as PluginBehaviac.Nodes.Condition;
            if (condition == null)
                return;

            if (condition.Opl != null)
            {
                RightValueCsExporter.GenerateClassMember(condition.Opl, stream, indent, "opl");
            }

            if (condition.Opr != null)
            {
                RightValueCsExporter.GenerateClassMember(condition.Opr, stream, indent, "opr");
            }
        }

        protected override void GenerateMethod(Node node, StreamWriter stream, string indent)
        {
            base.GenerateMethod(node, stream, indent);

            PluginBehaviac.Nodes.Condition condition = node as PluginBehaviac.Nodes.Condition;
            if (condition == null)
                return;

            stream.WriteLine("{0}\t\tprotected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)", indent);
            stream.WriteLine("{0}\t\t{{", indent);

            string typeName = DataCsExporter.GetGeneratedNativeType(condition.Opl.ValueType);

            // opl
            if (condition.Opl != null)
            {
                RightValueCsExporter.GenerateCode(condition.Opl, stream, indent + "\t\t\t", typeName, "opl", string.Empty);
                if (condition.Opl.IsMethod)
                    RightValueCsExporter.PostGenerateCode(condition.Opl, stream, indent + "\t\t\t", typeName, "opl", string.Empty);
            }

            // opr
            if (condition.Opr != null)
            {
                RightValueCsExporter.GenerateCode(condition.Opr, stream, indent + "\t\t\t", typeName, "opr", string.Empty);
                if (condition.Opr != null)
                    RightValueCsExporter.PostGenerateCode(condition.Opr, stream, indent + "\t\t\t", typeName, "opr", string.Empty);
            }

            // Operator
            switch (condition.Operator)
            {
                case OperatorType.Equal:
                    stream.WriteLine("{0}\t\t\tbool op = opl == opr;", indent);
                    break;

                case OperatorType.NotEqual:
                    stream.WriteLine("{0}\t\t\tbool op = opl != opr;", indent);
                    break;

                case OperatorType.Greater:
                    stream.WriteLine("{0}\t\t\tbool op = opl > opr;", indent);
                    break;

                case OperatorType.GreaterEqual:
                    stream.WriteLine("{0}\t\t\tbool op = opl >= opr;", indent);
                    break;

                case OperatorType.Less:
                    stream.WriteLine("{0}\t\t\tbool op = opl < opr;", indent);
                    break;

                case OperatorType.LessEqual:
                    stream.WriteLine("{0}\t\t\tbool op = opl <= opr;", indent);
                    break;

                case OperatorType.And:
                    stream.WriteLine("{0}\t\t\tbool op = opl && opr;", indent);
                    break;

                case OperatorType.Or:
                    stream.WriteLine("{0}\t\t\tbool op = opl || opr;", indent);
                    break;

                default:
                    stream.WriteLine("{0}\t\t\tbool op = false;", indent);
                    break;
            }

            stream.WriteLine("{0}\t\t\treturn op ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;", indent);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
