﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProcessManagement.Models;
namespace ProcessManagement.Services
{
	public class ParticipateService
	{
		///=============================================================================================
		PMSEntities db = new PMSEntities();
		///=============================================================================================



		/// <summary>
		/// Tạo mới một particition
		/// </summary>
		/// <param name="idUser">Id User</param>
		/// <param name="idGroup">Id Group</param>
		/// <param name="part">Model Participate</param>
		/// <param name="isOwner">Có phải owner hông??</param>
		public void createParticipate(string idUser, int idGroup, Participate part, bool isOwner)
		{

			part.IdGroup = idGroup;
			part.IdUser = idUser;
			if (isOwner)
			{
				part.IsOwner = true;
				part.IsAdmin = true;
				part.IsManager = true;
			}
			else
			{
				part.IsOwner = false;
				part.IsAdmin = false;
				part.IsManager = false;
			}
			part.Created_At = DateTime.Now;
			part.Updated_At = DateTime.Now;
			db.Participates.Add(part);
			db.SaveChanges();
		}
		/// <summary>
		/// Thay đổi role của một user
		/// </summary>
		/// <param name="model">Participate model</param>
		public void editRoleUser(Participate model)
		{
			var user = findMemberInGroup(model.Id);
			user.IsAdmin = model.IsAdmin;
			user.IsManager = model.IsManager;
			user.Updated_At = DateTime.Now;
			db.Entry(user).State = System.Data.Entity.EntityState.Modified;
			db.SaveChanges();
		}
		/// <summary>
		/// Xóa user ra khỏi group
		/// </summary>
		/// <param name="participate">Oarticipate Model</param>
		public void removeUserInGroup(Participate participate)
		{
			db.Participates.Remove(participate);
			db.SaveChanges();
		}
		public void removeUsersInGroup(List<Participate> listUser)
		{
			db.Participates.RemoveRange(listUser);
			db.SaveChanges();
		}
		/// <summary>
		/// Tìm một user thuộc group đó
		/// </summary>
		/// <param name="idParticipant">Id Participant</param>
		/// <returns>Thông tin user đó</returns>
		public Participate findMemberInGroup(int idParticipant)
		{
			Participate user = db.Participates.SingleOrDefault(x => x.Id == idParticipant);
			return user;
		}
		/// <summary>
		/// Tìm tất cả các member thuộc group do
		/// </summary>
		/// <param name="IdGroup">Id Group</param>
		/// <returns>Return danh sach các member thuộc group đó</returns>
		public List<Participate> findMembersInGroup(int IdGroup)
		{
			var ListParticipant = db.Participates.Where(x => x.IdGroup == IdGroup).ToList();
			return ListParticipant;
		}
		/// <summary>
		/// Tìm tất cả member không thuộc group đó
		/// </summary>
		/// <param name="memberInGroup">List các Members thuộc group đó</param>
		/// <returns>Return danh sách các Members không thuộc group đó</returns>
		public List<AspNetUser> findMembersNotInGroup(List<Participate> memberInGroup)
		{
			List<string> userInGroup = new List<string>();
			foreach (var item in memberInGroup)
			{
				userInGroup.Add(item.IdUser);
			}
			//string temp = String.Join(", ", userInGroup); 
			var memberNotInGroup = db.AspNetUsers.Where(x => !userInGroup.Contains(x.Id)).OrderByDescending(x => x.Id).ToList();
			return memberNotInGroup;
		}
		/// <summary>
		/// Lấy role của member trong một group
		/// </summary>
		/// <param name="idUser">Id Member</param>
		/// <param name="idGroup">Id Group</param>
		/// <returns>Return một object Participate của member thuộc group đó</returns>
		public Participate getRoleOfMember(string idUser, int idGroup)
		{
			var role = db.Participates.Where(x => x.IdUser == idUser && x.IdGroup == idGroup).FirstOrDefault();
			return role;
		}
		/// <summary>
		/// Add nhiều member vào group
		/// </summary>
		/// <param name="group">Model Group</param>
		/// <param name="listUser">Danh sách email của member</param>
		public void addMembers(Group group, List<string> listUser)
		{
			foreach (var user in listUser)
			{
				Participate role = new Participate();
				role.IdUser = db.AspNetUsers.SingleOrDefault(x => x.Email == user).Id;
				role.IdGroup = group.Id;
				role.Created_At = DateTime.Now;
				role.Updated_At = DateTime.Now;

				db.Participates.Add(role);
			}
			db.SaveChanges();
		}
		/// <summary>
		/// Tìm Role dựa trên id
		/// </summary>
		/// <param name="idRole">Id Role</param>
		/// <return>Return một object role</return>
		public Role findRole(int idRole)
		{
			Role role = db.Roles.Find(idRole);
			return role;
		}
		/// <summary>
		/// Tìm một role thuộc process đó
		/// </summary>
		/// <param name="idRole">Id Role</param>
		/// <returns>Thông tin role đó</returns>
		public Role findRoleInProcess(int idRole)
		{
			Role role = db.Roles.SingleOrDefault(x => x.Id == idRole);
			return role;
		}
		/// <summary>
		/// Tìm tất cả các role thuộc process đó
		/// </summary>
		/// <param name="IdProcess">Id Process</param>
		/// <returns>Return danh sach các member thuộc group đó</returns>
		public List<Role> findRolesInProcess(int IdProcess)
		{
			var ListRole = db.Roles.Where(x => x.IdProcess == IdProcess).ToList();
			return ListRole;
		}
		// <summary>
		/// Xóa role ra khỏi process
		/// </summary>
		/// <param name="role">Oarticipate Model</param>
		public void removeRoleInProcess(Role role)
		{
			db.Roles.Remove(role);
			db.SaveChanges();
		}
		public void removeRolesInProcess(List<Role> listRole)
		{
			db.Roles.RemoveRange(listRole);
			db.SaveChanges();
		}
		/// <summary>
		/// Thay đổi role của một process
		/// </summary>
		/// <param name="model">Role model</param>
		//public void editRoleInProcess(Role model)
		//{
		//	var role = findRoleInProcess(model.Id);
		//	db.Entry(role).State = System.Data.Entity.EntityState.Modified;
		//	db.SaveChanges();
		//}
		/// Edit thông tin role
		/// </summary>
		/// <param name = "model" > Role Model</param>
		public void editRole(Role model)
		{
			Role role = findRole(model.Id);
			role.Name = model.Name;
			role.Description = model.Description;
			db.SaveChanges();
		}
	}
}