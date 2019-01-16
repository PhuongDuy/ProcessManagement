﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProcessManagement.Models;
namespace ProcessManagement.Services
{
    public class StepService
    {
        ///=============================================================================================
        PMSEntities db = new PMSEntities();
        ///=============================================================================================


        /// <summary>
        /// Tìm tất cả các step cúa 1 process
        /// </summary>
        /// <param name="idProcess">Id Process</param>
        public List<Step> findStepsOfProcess(int idProcess)
        {
            List<Step> step = db.Steps.Where(x => x.IdProcess == idProcess).ToList();
            return step;
        }
        /// <summary>
        /// Xóa tất cả các step thuộc nhiều procses
        /// </summary>
        /// <param name="process">Process Model</param>
        public void removeSteps(List<Process> processes)
        {
            foreach (var process in processes)
            {
                var steps = findStepsOfProcess(process.Id);
                db.Steps.RemoveRange(steps);
            }
            db.SaveChanges();
        }
        public Step findStep(int id)
        {
            Step step = db.Steps.Find(id);
            return step;
        }
        public Step findStepByKey(int processid,int key)
        {
            Step step = db.Steps.Where(x => x.IdProcess == processid && x.Key == key).FirstOrDefault();
            return step;
        }
        public void removeStep(Step step)
        {
            db.Steps.Remove(step);
            db.SaveChanges();
        }
    }
}