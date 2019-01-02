﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Web;
using Microsoft.AspNet.Identity;
using ProcessManagement.Models;
using ProcessManagement.Services;
using ProcessManagement.Filters;
namespace ProcessManagement.Controllers
{
    
    public class GroupController : BaseController
    {
        ///=============================================================================================
        PMSEntities db = new PMSEntities();
        GroupService groupService = new GroupService();
        CommonService commonService = new CommonService();
        ParticipateService participateService = new ParticipateService();
        ProcessService processService = new ProcessService();
        ///=============================================================================================
        


        // GET: Group
        [Authorize]
        public ActionResult Index()
        {
            string IdUser = User.Identity.GetUserId();
            //lấy ra những group mà user đó tham gia hoặc sở hữu
            ViewData["ListGroup"] = groupService.getMyGroup(IdUser);
            return View();
        }
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Create(Group group, HttpPostedFileBase ImageGr)
        {

            string idUser = User.Identity.GetUserId();
            //save avatar
            commonService.saveAvatarGroup(group,ImageGr, Server.MapPath("~/Content/images/workspace/"));
            //create new group
            groupService.createGroup(idUser, group);
            //create new participate
            Participate part = new Participate();
            participateService.createParticipate(idUser, group.Id, part, true);

            //set flash
            SetFlash("Success", "Created Group Successfully");
            return RedirectToAction("Index", "Group");
        }
        [GroupAuthorize]
        public ActionResult Show(int id)
        {
            //Tìm group theo id
            Group group = groupService.findGroup(id);
            if (group == null) return HttpNotFound();

            Session["idGroup"] = group.Id;
            //Tìm tất cả member thuộc group đó
            var ListParticipant = participateService.findMembersInGroup(group.Id);
            ViewData["ListParticipant"] = ListParticipant;
            //Tìm tất cả các process thuộc group đó
            ViewData["ListProcess"] = processService.findProcess(group.Id);
            return View(group);
        }
        //public ActionResult AddMember(int? id)
        //{
        //    string userid = User.Identity.GetUserId();
        //    Group group = db.Groups.Find(id);
        //    if (group == null) return HttpNotFound();

        //    var ListParticipant = db.Participates.Where(x => x.IdGroup == id).ToList();
        //    ViewData["ListParticipant"] = ListParticipant;
        //    List<string> userInGroup = new List<string>();
        //    foreach (var item in ListParticipant)
        //    {
        //        userInGroup.Add(item.IdUser);
        //    }

        //    //string temp = String.Join(", ", userInGroup); 
        //    ViewData["ListUser"] = db.AspNetUsers.Where(x => !userInGroup.Contains(x.Id)).OrderByDescending(x => x.Id).ToList();
        //    Session["idGroup"] = id;
        //    return View(group);
        //}
        [Authorize]
        [HttpPost]
        public ActionResult AddMember(Group model, List<string> adduser)
        {
            Group group = groupService.findGroup(model.Id);
            if (group == null) return HttpNotFound();

            //Add members
            participateService.addMembers(model, adduser);
            TempData["UserSetting"] = "ABC";
            SetFlash("Success", "Added Members Successfully");
            return RedirectToAction("settings", new { id = model.Id });
        }
        [Authorize]
        //[RoleAuthorize("Admin")]
        public ActionResult Settings(int id)
        {
            string idUser = User.Identity.GetUserId();
            Group group = groupService.findGroup(id);
            if (group == null) return HttpNotFound();
            //Tìm tất cả member thuộc group đó
            var ListParticipant = participateService.findMembersInGroup(group.Id);
            ViewData["ListParticipant"] = ListParticipant;

            //Tìm tất cả member không thuộc group đó
            ViewData["ListUser"] = participateService.findMembersNotInGroup(ListParticipant);
            ViewData["Roles"] = participateService.getRoleOfMember(idUser, group.Id);
            return View(group);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Edit(Group model, HttpPostedFileBase ImageGr)
        {
            Group group = groupService.findGroup(model.Id);
            if (group == null) return HttpNotFound();

            commonService.saveAvatarGroup(model,ImageGr, Server.MapPath("~/Content/images/workspace/"));
            groupService.editGroup(model);
            TempData["GaneralSetting"] = "ABC";
            return RedirectToAction("Settings", new { id = model.Id });
        }
        [Authorize]
        [HttpGet]
        public ActionResult RemoveAvatar(Group model)
        {
            Group group = groupService.findGroup(model.Id);
            if (group == null) return HttpNotFound();
            groupService.removeAvatar(model);
            TempData["GaneralSetting"] = "ABC";
            SetFlash("Success", "Removed Avatar Successfully");
            return RedirectToAction("Settings", new { id = model.Id });
        }
        [Authorize]
        public ActionResult Remove(int id)
        {
            Group group = groupService.findGroup(id);
            var groupName = group.Name;
            ////////////////////////////////////////////////////////////////////////////
            //TODO
            //hỏi thầy lúc xóa group có nên xóa process luôn hông 
            //tại process này có thể là con của một process khác năng ở group khác
            ///////////////////////////////////////////////////////////////////////////
            
            //remove group (bao gồm remove participant,process,step)
            groupService.removeGroup(group);
            SetFlash("Success", "Removed Group "+ groupName + " Successfully");
            return RedirectToAction("Index");
        }
        
        [Authorize]
        public ActionResult DeleteMember(Participate model)
        {
            Participate user = participateService.findMemberInGroup(model.Id);
            if (user == null) return HttpNotFound();
            var userName = user.AspNetUser.UserName;
            var groupId = user.IdGroup;
            participateService.removeUserInGroup(user);
            TempData["UserSetting"] = "ABC";
            SetFlash("Success", "Removed "+ userName + " Successfully");
            return RedirectToAction("Settings", new { id = groupId });
        }
        [Authorize]
        public ActionResult EditRoleMember(int id)
        {
 
            Participate user = participateService.findMemberInGroup(id);
            if (user == null) return HttpNotFound();
            return View(user);
        }
        [Authorize]
        [HttpPost]
        public ActionResult EditRoleMember(Participate model)
        {
            Participate user = participateService.findMemberInGroup(model.Id);
            if (user == null) return HttpNotFound();
            //chỉnh sửa role của 1 user
            participateService.editRoleUser(model);

            var groupId = user.IdGroup;
            TempData["UserSetting"] = "ABC";
            SetFlash("Success", "Edited Role of "+user.AspNetUser.UserName+" Successfully");
            return RedirectToAction("Settings", new { id = groupId });
        }
        [Authorize]
        public ActionResult MemberLeaveGroup(Participate model)
        {
            Participate user = participateService.findMemberInGroup(model.Id);
            if (user == null) return HttpNotFound();
            var groupName = user.Group.Name;

            //xóa thành viên trong group
            participateService.removeUserInGroup(user);
            
            SetFlash("Success", "Left Group " + groupName + " Successfully");
            return RedirectToAction("Index");
        }
    }
}