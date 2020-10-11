using System;
using System.Reflection;

namespace WebAPI_Exercise_03_10_2020.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}