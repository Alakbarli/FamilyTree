#pragma checksum "C:\Users\Ulvi\source\repos\FamilyWithIdentity\GtsTask3Famly\Views\Home\Roles.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7a24b41d9a90ec1a72634d8f3a600608bcd17260"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Roles), @"mvc.1.0.view", @"/Views/Home/Roles.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/Roles.cshtml", typeof(AspNetCore.Views_Home_Roles))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\Ulvi\source\repos\FamilyWithIdentity\GtsTask3Famly\Views\_ViewImports.cshtml"
using GtsTask3Famly;

#line default
#line hidden
#line 2 "C:\Users\Ulvi\source\repos\FamilyWithIdentity\GtsTask3Famly\Views\_ViewImports.cshtml"
using GtsTask3Famly.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7a24b41d9a90ec1a72634d8f3a600608bcd17260", @"/Views/Home/Roles.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6ac63c005aa4c34559f0383fe2371ddd13e57323", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Roles : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<RelRole>>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "addRole", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-success"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-danger"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "home", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "removeRole", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "C:\Users\Ulvi\source\repos\FamilyWithIdentity\GtsTask3Famly\Views\Home\Roles.cshtml"
  
    ViewData["Title"] = "Roles";

#line default
#line hidden
            BeginContext(72, 25, true);
            WriteLiteral("\r\n    <div class=\"row\">\r\n");
            EndContext();
#line 8 "C:\Users\Ulvi\source\repos\FamilyWithIdentity\GtsTask3Famly\Views\Home\Roles.cshtml"
         if (TempData["addRole"] != null)
        {

#line default
#line hidden
            BeginContext(151, 126, true);
            WriteLiteral("            <div class=\"col-12 my-2\">\r\n                <p class=\"alert alert-success\">New Role Added</p>\r\n            </div>\r\n");
            EndContext();
#line 13 "C:\Users\Ulvi\source\repos\FamilyWithIdentity\GtsTask3Famly\Views\Home\Roles.cshtml"
        }

#line default
#line hidden
            BeginContext(288, 8, true);
            WriteLiteral("        ");
            EndContext();
#line 14 "C:\Users\Ulvi\source\repos\FamilyWithIdentity\GtsTask3Famly\Views\Home\Roles.cshtml"
         if (TempData["removeRole"] != null)
        {

#line default
#line hidden
            BeginContext(345, 124, true);
            WriteLiteral("            <div class=\"col-12 my-2\">\r\n                <p class=\"alert alert-success\">Role deleted</p>\r\n            </div>\r\n");
            EndContext();
#line 19 "C:\Users\Ulvi\source\repos\FamilyWithIdentity\GtsTask3Famly\Views\Home\Roles.cshtml"
        }

#line default
#line hidden
            BeginContext(480, 113, true);
            WriteLiteral("\r\n        <div class=\"col-12 mb-5\">\r\n            <div class=\"w-100 d-flex justify-content-end\">\r\n                ");
            EndContext();
            BeginContext(593, 64, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "7a24b41d9a90ec1a72634d8f3a600608bcd172606559", async() => {
                BeginContext(641, 12, true);
                WriteLiteral("Add new role");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(657, 368, true);
            WriteLiteral(@"
            </div>
            

        </div>
        <div class=""col-12"">
            <table class=""table table-striped"">
                <thead>
                    <tr>
                        <th scope=""col"">Name</th>
                        <th scope=""col"">Delete</th>

                    </tr>
                </thead>
                <tbody>
");
            EndContext();
#line 38 "C:\Users\Ulvi\source\repos\FamilyWithIdentity\GtsTask3Famly\Views\Home\Roles.cshtml"
                     foreach (var item in Model)
                    {

#line default
#line hidden
            BeginContext(1098, 62, true);
            WriteLiteral("                        <tr>\r\n                            <td>");
            EndContext();
            BeginContext(1161, 9, false);
#line 41 "C:\Users\Ulvi\source\repos\FamilyWithIdentity\GtsTask3Famly\Views\Home\Roles.cshtml"
                           Write(item.Name);

#line default
#line hidden
            EndContext();
            BeginContext(1170, 39, true);
            WriteLiteral("</td>\r\n                            <td>");
            EndContext();
            BeginContext(1209, 106, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "7a24b41d9a90ec1a72634d8f3a600608bcd172609145", async() => {
                BeginContext(1305, 6, true);
                WriteLiteral("Delete");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_4.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#line 42 "C:\Users\Ulvi\source\repos\FamilyWithIdentity\GtsTask3Famly\Views\Home\Roles.cshtml"
                                                                                                          WriteLiteral(item.Id);

#line default
#line hidden
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(1315, 38, true);
            WriteLiteral("</td>\r\n                        </tr>\r\n");
            EndContext();
#line 44 "C:\Users\Ulvi\source\repos\FamilyWithIdentity\GtsTask3Famly\Views\Home\Roles.cshtml"
                    }

#line default
#line hidden
            BeginContext(1376, 86, true);
            WriteLiteral("\r\n\r\n\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n\r\n    </div>\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<RelRole>> Html { get; private set; }
    }
}
#pragma warning restore 1591
