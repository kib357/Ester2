using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using EsterCommon.BaseClasses;
using EsterServer.Modules.Users.Database;
using EsterServer.Modules.Users.Wrappers;

namespace EsterServer.Modules.Users.ActiveDirectory
{
    public static class WorkWithAD
    {
        public static List<DomainTree> GetDomainList(string forestAddress, string login, string password)
        {
            try
            {
                Forest forest = Forest.GetForest(new DirectoryContext(DirectoryContextType.Forest, forestAddress, login, password));
                DomainCollection myDomains = forest.Domains;
                return GetDomainTree(myDomains);
            }
            catch (Exception ex)
            {
        
                throw ex;
            }                       
        }

        private static List<DomainTree> GetDomainTree(DomainCollection forest)
        {
            return (from Domain domain in forest select new DomainTree() {Name = domain.Name, Subdomains = GetDomainTree(domain.Children)}).ToList();
        }

        public static List<string> GetUsers(string domain, string login, string password)
        {
            var users = new List<string>();
            try
            {
                var en = new DirectoryEntry("LDAP://"+domain, login, password);
                var srch = new DirectorySearcher(en)
                               {Filter = "(&(objectClass=user))", SearchScope = SearchScope.Subtree};
                SearchResultCollection coll = srch.FindAll();
                users.AddRange(from SearchResult rs in coll select rs.GetDirectoryEntry().Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return users;
        }

        public static DirectoryEntry GetUserInfo(string domain, string login, string password, string name)
        {
            DirectoryEntry user;
            try
            {
                var en = new DirectoryEntry("LDAP://" + domain, login, password);
                var srch = new DirectorySearcher(en) { Filter = "(&(objectClass=user))", SearchScope = SearchScope.Subtree };
                SearchResultCollection coll = srch.FindAll();
                user=(from SearchResult rs in coll where rs.GetDirectoryEntry().Name==name select rs.GetDirectoryEntry()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return user;
        }

        public static void AddUser(User user)
        { 
            WorkWithXmlDB.AddUser(user);
        }

    }
}
