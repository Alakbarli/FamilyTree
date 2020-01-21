using GtsTask3Famly.DAL;
using GtsTask3Famly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GtsTask3Famly.Utilities
{
    public static class FamlyMethods
    {
        public static void AddRelation(DB dB, int RelationUserId, int personId, int RoleId, int FamilyId)
        {
            if (dB.Relationships.FirstOrDefault(x => x.RelatedUserId == RelationUserId && x.PersonId == personId && x.FamilyId == FamilyId) == null)
            {
                Relationship newRelation = new Relationship
                {
                    RelatedUserId = RelationUserId,
                    PersonId = personId,
                    RelRoleId = RoleId,
                    FamilyId = FamilyId
                };
                dB.Relationships.Add(newRelation);
            }
        }

        public static void CreatePartial(IEnumerable<Relationship> Childs, IEnumerable<Relationship> AllRelationship, ref string Html, bool main)
        {

            if (main)
            {
                foreach (var item in Childs)
                {
                    IEnumerable<Relationship> Newrel = new List<Relationship>();
                    IEnumerable<Relationship> Children = new List<Relationship>();
                    Newrel = AllRelationship.Where(x => x.RelatedUserId == item.RelatedUserId);
                    Relationship Spouse = Newrel.FirstOrDefault(x => x.Role.Name.ToLower() == "husband" || x.Role.Name.ToLower() == "wife");
                    string genrerClass = item.RelatedUser.GenderId == 1 ? "maleperson" : "femaleperson";
                    bool hasChildren = Newrel.Any(x => (x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"));
                    Children = Newrel.Where((x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"));
                    //Crate Person profile
                    Html += $"<ul><li>";
                    Html +="<div class=\"personcontainer\">" +
             $"<a data-id=\"{item.RelatedUser.Id}\" data-familyId=\"{item.FamilyId}\" class=\"persondiv {genrerClass} \">" +
                  "<div class=\"image\">" +
                   $"<img src ={item.RelatedUser.Photo}>" +
                 " </div>" +
                 " <div class=\"name\">" +
                    $"<span >" +
                     $" {item.RelatedUser.Firstname+" "+item.RelatedUser.LastName}" +
                   " </span>" +
                 " </div>  </a> ";

                    if (Spouse != null)
                    {
                        string SpousegenderClass = Spouse.Person.GenderId == 1 ? "maleperson" : "femaleperson";
                        Html += "" +
                        $"<a data-id=\"{Spouse.Person.Id}\" data-familyId=\"{Spouse.FamilyId}\" class=\"persondiv {SpousegenderClass}\">" +
                         "<div class=\"image\">" +
                          $" <img src ={Spouse.Person.Photo}>" +
                         "</div>" +
                        "<div class=\"name\">" +
                         $"<span>" +
                          $"{Spouse.Person.Firstname+Spouse.Person.LastName}" +
                          "</span>" +
                         "</div>  " +
                     "</a>";

                    }
                    if (hasChildren)
                    {
                        Html += "<div title=\"Show children\" class=\"morePerson\"><i class=\"fad fa-arrow-down\"></i></div>";
                    }
                    Html += " </div>";
                    if (hasChildren)
                    {
                        Html += "<ul>";
                        CreatePartial(Children, AllRelationship, ref Html,false);
                        Html += "</ul>";
                    }

                    Html += "</li></ul>";
                }
                return;
            }
            else
            {
                foreach (var item in Childs)
                {
                    IEnumerable<Relationship> Newrel = new List<Relationship>();
                    IEnumerable<Relationship> Children = new List<Relationship>();
                    Newrel = AllRelationship.Where(x => x.RelatedUserId == item.PersonId);
                    Relationship Spouse = Newrel.FirstOrDefault(x => x.Role.Name.ToLower() == "husband" || x.Role.Name.ToLower() == "wife");
                    Children = Newrel.Where((x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"));
                    string genrerClass = item.Person.GenderId == 1 ? "maleperson" : "femaleperson";
                    bool hasChildren = Newrel.Any(x => (x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"));
                    //Crate Person profile
                    Html += $"<li>";
                    Html += "<div class=\"personcontainer\">" +
             $"<a data-id=\"{item.Person.Id}\" data-familyId=\"{item.FamilyId}\" class=\"persondiv {genrerClass} \">" +
                  "<div class=\"image\">" +
                   $"<img src ={item.Person.Photo}>" +
                 "</div>" +
                 "<div class=\"name\">" +
                    $"<span data-id=\"{item.Person.Id}\" data-familyId=\"{item.FamilyId}\">" +
                     $" {item.Person.Firstname + " " + item.Person.LastName}" +
                   "</span>" +
                 " </div>  </a> ";
                    if (Spouse != null)
                    {
                        string SpousegenderClass = Spouse.Person.GenderId == 1 ? "maleperson" : "femaleperson";
                        Html += "" +
                        $"<a data-id=\"{Spouse.Person.Id}\" data-familyId=\"{Spouse.FamilyId}\" class=\"persondiv {SpousegenderClass}\">" +
                         "<div class=\"image\">" +
                          $" <img src ={Spouse.Person.Photo}>" +
                         "</div>" +
                        "<div class=\"name\">" +
                         $"<span data-id=\"{Spouse.Person.Id}\" data-familyId=\"{Spouse.FamilyId}\">" +
                          $"{Spouse.Person.Firstname + Spouse.Person.LastName}" +
                          "</span>" +
                         "</div>  " +
                     "</a>";
                    }
                    if (hasChildren)
                    {
                        Html+= "<div title=\"Show children\" class=\"morePerson\">"+
                            "<i class=\"fad fa-arrow-down\"></i></div>";
                    }
                    Html += " </div>";
                    if (hasChildren)
                    {
                        Html += "<ul>";
                        CreatePartial(Children, AllRelationship, ref Html,false);
                        Html += "</ul>";
                    }
                    Html += "</li>";
                }
                return;
            }


        }
        public static int GetFamilyId(DB db,User user)
        {
            FamilyToUser familyToUser = db.FamilyToUsers.FirstOrDefault(x => x.UserId == user.Id);
            return familyToUser.FamilyId;
        }
    }
}
