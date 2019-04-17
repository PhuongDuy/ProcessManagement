﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using ProcessManagement.Filters;
using ProcessManagement.Services;
using ProcessManagement.Models;
namespace ProcessManagement.Controllers
{
    public class ProcessRunController : Controller
    {
        ///=============================================================================================
        PMSEntities db = new PMSEntities();
        GroupService groupService = new GroupService();
        CommonService commonService = new CommonService();
        ParticipateService participateService = new ParticipateService();
        ProcessService processService = new ProcessService();
        StepService stepService = new StepService();
        TaskService taskService = new TaskService();
        RoleService roleService = new RoleService();
        FileService fileService = new FileService();
        ///=============================================================================================


        // GET: ProcessRun
        [GroupAuthorize]
        public ActionResult AssignRole(int processid)
        {
            //TODO: chỉ được lấy process đang run thôi,process thường không dc assign role
            var processRun = processService.findProcess(processid);
            if (processRun == null) return HttpNotFound();

            var listRole = roleService.findListRoleOfProcess(processid);
            var listUserInGroup = participateService.findMembersInGroup(processRun.IdGroup);

            List<object> jRoleList = new List<object>();
            foreach (var role in listRole)
            {
                List<object> jMemberList = new List<object>();
                foreach (var user in listUserInGroup)
                {
                    object jMeber = new
                    {
                        id = user.AspNetUser.Id,
                        name = user.AspNetUser.UserName,
                        isAssigned = roleService.isAssigned(role.Id, user.AspNetUser.Id)
                    };
                    jMemberList.Add(jMeber);
                }
                object jRole = new
                {
                    id = role.Id,
                    name = role.Name,
                    description = role.Description,
                    users = jMemberList
                };
                jRoleList.Add(jRole);
            }

            ViewData["ProcessRun"] = processRun;
            ViewData["ListRole"] = listRole;
            ViewData["ListUser"] = listUserInGroup;
            ViewData["Roles"] = JArray.FromObject(jRoleList);
            return View(processRun);
        }

        [GroupAuthorize]
        public ActionResult Detail(int processid)
        {
            string idUser = User.Identity.GetUserId();
            var processrun = processService.findProcess(processid,true);
            if (processrun == null) return HttpNotFound();

            var listrole = roleService.findListRoleOfProcess(processid);
            var group = groupService.findGroup(processrun.IdGroup);
            var listroleruns = roleService.findlistrolerun(listrole);
            var listStep = stepService.findStepsOfProcess(processid);
            var runprocess = processService.findRunProcessbyidprorun(processrun.Id);
            var ktra = db.ProcessRuns.Where(x => x.IdProcess == processrun.Id).FirstOrDefault();
            List<StepRun> liststepofrunprocess = new List<StepRun>();
            if (runprocess != null)
            {
                liststepofrunprocess = stepService.findStepsOfRunProcess(runprocess.Id);
            }


            List<Step> listnextstep1 = new List<Step>();
            List<Step> listnextstep2 = new List<Step>();
            Step start = listStep.Where(x => x.StartStep == true).FirstOrDefault();
            listnextstep1.Add(start);
            int z = 0;
            int t = 0;
            for (int j = 0; j < listStep.Count; j++)
            {
                if (listnextstep1[j].NextStep1 == 0)
                {
                    break;
                }
                do
                {
                    if (listStep[z].Key == listnextstep1[j].NextStep2 && listStep[z].StartStep == false)
                    {
                        listnextstep2.Add(listStep[z]);
                        z = 0;
                        break;
                    }
                    if (listStep[z].Key == listnextstep1[j].NextStep1 && listStep[z].StartStep == false)
                    {
                        listnextstep1.Add(listStep[z]);
                        if (listnextstep1[j].Figure == "Diamond")
                        {
                        }
                        else
                        {
                            z = 0;
                            break;
                        }
                    }
                    z++;
                } while (z < listStep.Count);
            }
            if (listnextstep2.Count() > 0)
            {
                for (int j = 0; j < listStep.Count; j++)
                {
                    if (listnextstep2[j].NextStep1 == 0)
                    {
                        break;
                    }
                    do
                    {
                        if (listStep[t].Key == listnextstep2[j].NextStep1 && listStep[t].StartStep == false)
                        {
                            listnextstep2.Add(listStep[t]);
                            t = 0;
                            break;
                        }
                        t++;
                    } while (t < listStep.Count);
                }
            }
            foreach (var item in listnextstep2)
            {
                listnextstep1.Add(item);
            }
            StepRun runnextstep = new StepRun();
            foreach (StepRun steprun in liststepofrunprocess.Where(x => x.Status1.Name == "Running"))
            {
                runnextstep = steprun;
            }

            //get processrun file 
            string processRunPath = string.Format("Upload/{0}/{1}", group.Id, processrun.Id);
            List<FileManager> files = fileService.findFiles(group.Id, processRunPath);

            ViewData["ListRole"] = listrole;
            ViewData["ProcessRun"] = processrun;
            ViewData["ListRoleRuns"] = listroleruns;
            ViewBag.ListRunStep = liststepofrunprocess.Where(x => x.Figure == "Step");
            ViewBag.Checkprocessrun = ktra;
            ViewData["StepisNext"] = runnextstep;
            ViewData["UserRoles"] = participateService.getRoleOfMember(idUser, group.Id);
            //file 
            ViewData["Files"] = files;
            return View(listnextstep1);
        }

        [GroupAuthorize]
        public ActionResult Detailtask(int idruntask)
        {
            string idUser = User.Identity.GetUserId();
            TaskProcessRun taskrun = taskService.findTaskRun(idruntask);
            if (taskrun == null) return HttpNotFound();
            if (taskrun.ValueFormJson != null)
            {
                return RedirectToRoute("GroupControlLocalizedDefault", new { controller = "processrun", action = "Detailtaskform", taskid = idruntask });
            }
            var listrolenotrun = roleService.findListRoleOfProcess(taskrun.StepRun.ProcessRun.IdProcess);
            var listroleruns = roleService.findlistrolerun(listrolenotrun);
            RoleRun role = new RoleRun();
            foreach (var item in listroleruns)
            {
                if (taskrun.IdRole != null)
                {
                    if (idUser == item.IdUser && item.Role.Id == taskrun.Role.Id)
                    {
                        role = item;
                    }
                }
            }
            //get taskrun file 
            var groupId = taskrun.StepRun.ProcessRun.Process.IdGroup;
            var processrunId = taskrun.StepRun.ProcessRun.Process.Id;
            var stepId = taskrun.StepRun.CloneFrom;
            var taskId = taskrun.CloneForm;
            string taskRunPath = string.Format("Upload/{0}/{1}/{2}/{3}", groupId, processrunId, stepId, taskId);
            List<FileManager> files = fileService.findFiles(groupId, taskRunPath);
            string userTaskRunPath = string.Format("Upload/{0}/run/{1}/{2}/{3}", groupId, taskrun.StepRun.idProcess, taskrun.IdStep, taskrun.Id);
            FileManager userFile = fileService.findFile(groupId, userTaskRunPath);

            ViewData["Rolerun"] = role;
            ViewData["UserId"] = idUser;
            ViewData["ValueInput"] = JObject.Parse(taskrun.ValueInputText);
            ViewData["ValueFile"] = JObject.Parse(taskrun.ValueInputFile);
            ViewData["UserRoles"] = participateService.getRoleOfMember(idUser, taskrun.StepRun.ProcessRun.Process.IdGroup);
            //file 
            ViewData["FileMaxSize"] = db.ConfigRules.Find("filesize");
            ViewData["TextMaxLength"] = db.ConfigRules.Find("textlength");
            ViewData["Files"] = files;
            ViewData["UserFile"] = userFile;
            return View(taskrun);
        }

        [GroupAuthorize]
        public ActionResult Detailtaskform(int idruntask)
        {
            string idUser = User.Identity.GetUserId();
            TaskProcessRun taskrun = taskService.findTaskRun(idruntask);
            if (taskrun.ValueFormJson == null)
            {
                return RedirectToRoute("GroupControlLocalizedDefault", new { controller = "processrun", action = "Detailtask", taskid = idruntask });
            }
            var listrolenotrun = roleService.findListRoleOfProcess(taskrun.StepRun.ProcessRun.IdProcess);
            var listroleruns = roleService.findlistrolerun(listrolenotrun);
            RoleRun role = new RoleRun();
            foreach (var item in listroleruns)
            {
                if (taskrun.IdRole != null)
                {
                    if (idUser == item.IdUser && item.Role.Id == taskrun.Role.Id)
                    {
                        role = item;
                    }
                }
            }

            //get taskrun file 
            var groupId = taskrun.StepRun.ProcessRun.Process.IdGroup;
            var processrunId = taskrun.StepRun.ProcessRun.Process.Id;
            var stepId = taskrun.StepRun.CloneFrom;
            var taskId = taskrun.CloneForm;
            string processRunPath = string.Format("Upload/{0}/{1}/{2}/{3}", groupId, processrunId, stepId, taskId);
            List<FileManager> files = fileService.findFiles(groupId, processRunPath);

            ViewData["Rolerun"] = role;
            ViewData["UserRoles"] = participateService.getRoleOfMember(idUser, taskrun.StepRun.ProcessRun.Process.IdGroup);
            ViewData["UserId"] = idUser;
            //file 
            ViewData["Files"] = files;
            ViewData["FileMaxSize"] = db.ConfigRules.Find("filesize");
            ViewData["TextMaxLength"] = db.ConfigRules.Find("textlength");
            return View(taskrun);
        }
    }
}