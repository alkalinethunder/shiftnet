﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShiftOS.Server.WebAdmin.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ShiftOS.Server.WebAdmin.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;h3&gt;Create/edit chat&lt;/h3&gt;
        ///
        ///&lt;p&gt;Please fill out the details below for your channel list to be modified.&lt;/p&gt;
        ///
        ///{form}.
        /// </summary>
        internal static string ChatEditTemplate {
            get {
                return ResourceManager.GetString("ChatEditTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;h3&gt;Chats&lt;/h3&gt;
        ///
        ///&lt;p&gt;On this page you can find a list of all chats in the system. Chats are a part of the multi-user domain that allows online players to talk to eachother in the &apos;MUD Chat&apos; application.&lt;/p&gt;
        ///
        ///&lt;p&gt;If you have a Discord server for your multi-user domain, you can also designate a ShiftOS chat to listen on a specific channel on your server. You will need to create a Discord Bot Token and specify the ID of the channel you want tolisten to.&lt;/p&gt;
        ///
        ///&lt;p&gt;Once the chat is set up, you should see a bot  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ChatListView {
            get {
                return ResourceManager.GetString("ChatListView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;h3&gt;{listtitle}&lt;/h3&gt;
        ///
        ///&lt;p&gt;{listdesc}&lt;/p&gt;
        ///
        ///{list}.
        /// </summary>
        internal static string GenericTableList {
            get {
                return ResourceManager.GetString("GenericTableList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;html&gt;
        ///	&lt;head&gt;
        ///		&lt;title&gt;Multi-user domain &amp;bull; ShiftOS&lt;/title&gt;
        ///		&lt;link rel=&quot;stylesheet&quot; href=&quot;https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css&quot; integrity=&quot;sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u&quot; crossorigin=&quot;anonymous&quot;&gt;
        ///
        ///		&lt;link rel=&quot;stylesheet&quot; href=&quot;http://getshiftos.ml/css/theme.css&quot;/&gt;
        ///
        ///		&lt;!-- Latest compiled and minified JavaScript --&gt;
        ///		&lt;script src=&quot;https://code.jquery.com/jquery-3.1.1.js&quot; integrity=&quot;sha256-16cdPddA6VdVInumRGo6IbivbERE8p7C [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string HtmlTemplate {
            get {
                return ResourceManager.GetString("HtmlTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;h3&gt;Access denied.&lt;/h3&gt;
        ///
        ///&lt;p&gt;You require a higher authentication level to access this part of the multi-user domain. Please enter the username and password of whom has access to this sector.&lt;/p&gt;
        ///
        ///&lt;form method=&quot;post&quot; action=&quot;&quot;&gt;
        ///	&lt;table class=&quot;table&quot;&gt;
        ///		&lt;tr&gt;
        ///			&lt;td&gt;&lt;strong&gt;Username:&lt;/strong&gt;&lt;/td&gt;
        ///			&lt;td&gt;&lt;input class=&quot;form-control&quot; type=&quot;text&quot; name=&quot;username&quot;/&gt;&lt;/td&gt;
        ///		&lt;/tr&gt;
        ///		&lt;tr&gt;
        ///			&lt;td&gt;&lt;strong&gt;Password:&lt;/strong&gt;&lt;/td&gt;
        ///			&lt;td&gt;&lt;input class=&quot;form-control&quot; type=&quot;password&quot; name=&quot;password&quot;/&gt;&lt;/td&gt;
        ///		&lt;/tr [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string LoginView {
            get {
                return ResourceManager.GetString("LoginView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;h3&gt;No users found.&lt;/h3&gt;
        ///
        ///&lt;p&gt;Your multi-user domain is newly-created. Before you can use the admin panel, you must create a ShiftOS user to act as the administrator of the MUD.&lt;/p&gt;
        ///
        ///{user_create_form}.
        /// </summary>
        internal static string NoUsersFound {
            get {
                return ResourceManager.GetString("NoUsersFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;h1&gt;Initial setup&lt;/h1&gt;
        ///
        ///&lt;p&gt;This multi-user domain contains some users, however none of them are administrators. Please choose your user to make it an admin.&lt;/p&gt;
        ///
        ///{savelist}.
        /// </summary>
        internal static string SetupView {
            get {
                return ResourceManager.GetString("SetupView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;h3&gt;System status&lt;/h3&gt;
        ///
        ///&lt;p&gt;Below is a summary of this multi-user domain&apos;s status.&lt;/p&gt;
        ///
        ///&lt;div class=&quot;row&quot;&gt;
        ///	&lt;div class=&quot;col-xs-6&quot;&gt;
        ///		&lt;h4&gt;MUD stats&lt;/h4&gt;
        ///		&lt;ul&gt;
        ///			&lt;li&gt;This server is worth &lt;strong&gt;{cp_worth}&lt;/strong&gt; Codepoints.&lt;/li&gt;
        ///			&lt;li&gt;This server has &lt;strong&gt;{user_count}&lt;/strong&gt; players registered.&lt;/li&gt;
        ///		&lt;/ul&gt;
        ///	&lt;/div&gt;
        ///	&lt;div class=&quot;col-xs-6&quot;&gt;
        ///		&lt;h4&gt;System environment&lt;/h4&gt;
        ///		&lt;ul&gt;
        ///			&lt;li&gt;&lt;strong&gt;Current system time:&lt;/strong&gt; {system_time}&lt;/li&gt;
        ///		&lt;/ul&gt;
        ///	&lt;/div&gt;
        ///&lt;/div&gt;.
        /// </summary>
        internal static string Status {
            get {
                return ResourceManager.GetString("Status", resourceCulture);
            }
        }
    }
}