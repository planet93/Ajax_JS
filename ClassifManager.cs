using Phoenix.Domain.Model;
using Phoenix.Infrastructure.Business;
using Phoenix.Infrastructure.Data;
using Phoenix.Infrastructure.Data.Native;
using Phoenix.Infrastructure.ViewModel.Classifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Web.Logic {
	public class ClassifManager {
		public List<ClassifierViewModel> GetClassifierModelList(long classifTypeId) {
			var u = new UnitOfWork ();
			var classifType = u.ClassifierType.GetActive().FirstOrDefault(w => w.Id == classifTypeId);
			var sql = $@"SELECT  C.[Id]
								, C.[Name]
								, C.[Active]
								, C.[Code]
								, C.[Year]
								, CT.Name AS Type
								, CT.RusName AS TypeRus
								, C.ClassifierType_Id AS TypeId
							   , L2.Name AS Level2
							  , CASE CT.Name
									WHEN 'CurrencyCourse' THEN CAST(C.[Year] AS nvarchar(25))
									ELSE L1.Name    
								 END  AS Level1 

							  FROM [dbo].[Classifier] AS C
							  LEFT JOIN [ClassifierType] AS CT
							  ON C.ClassifierType_Id = CT.Id
							  LEFT JOIN [Classifier] AS L2
							  ON C.Parent_Id = L2.Id
							  LEFT JOIN [Classifier] AS L1
							  ON L2.Parent_Id = L1.Id

								WHERE C.ClassifierType_Id = {classifTypeId} AND C.Deleted = 0
								Order by L1.Name, L2.Name, C.[Name]";

			var ddm = new DirectDataManager(sql);
			var list = ddm.ToList<ClassifierViewModel>().ToList();

			if(classifType.Name == "CurrencyCourse") {
				list = list.OrderByDescending(o => o.Year).ToList();
			}
			return list;
		}

		public List<ClassifierViewModel> GetInternalOrderModelList(long classifTypeId) {
			var u = new UnitOfWork();
			//var classifType = u.ClassifierType.GetActive().FirstOrDefault(w => w.Id == classifTypeId);
			var sql = $@"SELECT  C.[Id]
								, C.[Name]
								, C.[Active]
								, C.[Code]
								, CT.Name AS Type
								, CT.RusName AS TypeRus
								, C.ClassifierType_Id AS TypeId
							  , L2.Name AS Level2
							  , L1.Name AS Level1

							  FROM [dbo].[Classifier] AS C
							  LEFT JOIN [ClassifierType] AS CT
							  ON C.ClassifierType_Id = CT.Id
							   LEFT JOIN [IOMapping] AS IO
							  ON C.Id = IO.InternalOrder_Id
							  LEFT JOIN [Classifier] AS L2
							  ON IO.Brand_Id = L2.Id
							  LEFT JOIN [Classifier] AS L1
							  ON IO.Client_Id = L1.Id

								WHERE C.ClassifierType_Id = {classifTypeId} AND C.Deleted = 0
								Order by L1.Name, L2.Name, C.[Name]";

			var ddm = new DirectDataManager(sql);
			var list = ddm.ToList<ClassifierViewModel>().ToList();

			return list;
		}

		public ClassifierViewModel GetInternalOrder(long classifId, string type = "") {
			var u = new UnitOfWork();
			var classifType = u.ClassifierType.GetActive().FirstOrDefault(w => w.Name == type);
			var sql = $@"SELECT  C.[Id]
								, C.[Name]
								, C.[Active]
								, C.[Code]
								, CT.Name AS Type
								, CT.RusName AS TypeRus
								, C.ClassifierType_Id AS TypeId
							  , L2.Name AS Level2
							  , L1.Name AS Level1
							  , L2.Id AS Level2Id
							  , L1.Id AS Level1Id

							  FROM [dbo].[Classifier] AS C
							  LEFT JOIN [ClassifierType] AS CT
							  ON C.ClassifierType_Id = CT.Id
							   LEFT JOIN [IOMapping] AS IO
							  ON C.Id = IO.InternalOrder_Id
							  LEFT JOIN [Classifier] AS L2
							  ON IO.Brand_Id = L2.Id
							  LEFT JOIN [Classifier] AS L1
							  ON IO.Client_Id = L1.Id

								WHERE C.Id = {classifId} AND C.Deleted = 0
								Order by L1.Name, L2.Name, C.[Name]";

			var ddm = new DirectDataManager(sql);
			var list = ddm.ToList<ClassifierViewModel>().ToList();
			if (list.Count == 0) {
				var newClassifier = new ClassifierViewModel();
				newClassifier.Active = true;
				newClassifier.TypeId = classifType.Id;
				newClassifier.Type = classifType.Name;
				newClassifier.TypeRus = classifType.RusName;
				list.Add(newClassifier);
			}

			return list.Count == 0 ? null : list[0];
		}

		public ClassifierViewModel GetClassifierModel(long classifId, string type = "") {
			var u = new UnitOfWork();
			var classifType = u.ClassifierType.GetActive().FirstOrDefault(w => w.Name == type);
			var sql = $@"SELECT  C.[Id]
									, C.[Name]
									, C.[Active]
									, C.[Code]
									, C.[Year]
									, L2.Id AS CurrencyId
									, L2.Id AS Level2Id
									, L2.Name AS Level2
									, L1.Id AS Level1Id
									, L1.Name AS Level1
									, T.Name AS Type
									, T.RusName AS TypeRus
									, C.ClassifierType_Id AS TypeId

							  FROM [dbo].[Classifier] AS C 
							  LEFT JOIN [ClassifierType] AS T
							  ON C.ClassifierType_Id = T.Id
							  LEFT JOIN [Classifier] AS L2
							  ON C.Parent_Id = L2.Id
							  LEFT JOIN [Classifier] AS L1
							  ON L2.Parent_Id = L1.Id

								WHERE C.Id = {classifId} AND C.Deleted = 0
								Order by L1.Name, L2.Name, C.[Name]";

			var ddm = new DirectDataManager(sql);
			var list = ddm.ToList<ClassifierViewModel>().ToList();
			if (list.Count == 0) {
				var newClassifier = new ClassifierViewModel();
				newClassifier.Active = true;
				newClassifier.TypeId = classifType.Id;
				newClassifier.Type = classifType.Name;
				newClassifier.TypeRus = classifType.RusName;
				list.Add(newClassifier);
			}

			return list[0];
		}

		public void SaveClassifierModel(ClassifierViewModel model) {
			var u = new UnitOfWork();
			if (model.Id > 0) {
				var classif = u.Classifier.GetAll().FirstOrDefault(w => w.Id == model.Id);
				classif.Name = model.Name;
				classif.Code = model.Code;
				classif.Active = model.Active;
				if (model.Level2Id > 0 && (classif.Parent.Id != model.Level2Id)) {
					var parent = u.Classifier.GetAll().FirstOrDefault(w => w.Id == model.Level2Id);
					if (parent != null) {
						classif.Parent = parent;
					}
				}
				else {
					if (model.Level1Id > 0 && (classif.Parent.Id != model.Level1Id)) {
						var parent = u.Classifier.GetAll().FirstOrDefault(w => w.Id == model.Level1Id);
						if (parent != null) {
							classif.Parent = parent;
						}
					}
				}

				u.Classifier.Edit(classif);
				u.Classifier.Save();
			}
			else {
				var type = u.ClassifierType.GetAll().FirstOrDefault(w => w.Name == model.Type);
				if (type == null) return;
				var newClassif = new Classifier() { Name = model.Name, Code = model.Code, Active = model.Active, ClassifierType = type, DateFrom = DateTime.Now, DateTo = DateTime.Now };
				if (model.Level2Id > 0) {
					var parent = u.Classifier.GetAll().FirstOrDefault(w => w.Id == model.Level2Id);
					if (parent != null) {
						newClassif.Parent = parent;
					}
				}
				else {
					if (model.Level1Id > 0) {
						var parent = u.Classifier.GetAll().FirstOrDefault(w => w.Id == model.Level1Id);
						if (parent != null) {
							newClassif.Parent = parent;
						}
					}
				}
				u.Classifier.Add(newClassif);
				u.Classifier.Save();
			}
		}

		public void SaveInternalOrder(ClassifierViewModel model) {
			var u = new UnitOfWork();
			if (model.Id > 0) {
				var io = u.Classifier.GetAll().FirstOrDefault(w => w.Id == model.Id);
				var ioMapping = u.IOMapping.GetAll().FirstOrDefault(f => f.InternalOrder.Id == io.Id);
				if (io != null){
					if(io.Name != model.Name || io.Code != model.Code || io.Active != model.Active){
						io.Name = model.Name;
						io.Code = model.Code;
						io.Active = model.Active;

						u.Classifier.Edit(io);
						u.Classifier.Save();
					}
					
					if(ioMapping != null && (ioMapping.Brand.Id != model.Level2Id || ioMapping.Client.Id != model.Level1Id)) {
						var brand = u.Classifier.GetAll().FirstOrDefault(w => w.Id == model.Level2Id);
						var client = u.Classifier.GetAll().FirstOrDefault(w => w.Id == model.Level1Id);
						ioMapping.Brand = brand;
						ioMapping.Client = client;
						ioMapping.Active = model.Active;
						u.IOMapping.Edit(ioMapping);
						u.IOMapping.Save();
					}
				}
			}
			else{
				var type = u.ClassifierType.GetAll().FirstOrDefault(w => w.Name == model.Type);
				if (type == null) return;
				var newClassif = new Classifier() { Name = model.Name, Code = model.Code, Active = model.Active, ClassifierType = type, DateFrom = DateTime.Now, DateTo = DateTime.Now };
				u.Classifier.Add(newClassif);
				u.Classifier.Save();
				var brand = u.Classifier.GetAll().FirstOrDefault(w => w.Id == model.Level2Id);
				var client = u.Classifier.GetAll().FirstOrDefault(w => w.Id == model.Level1Id);
				var newIOMapping = new IOMapping() {InternalOrder = newClassif, Client = client, Brand = brand, Active = model.Active };
				u.IOMapping.Add(newIOMapping);
				u.IOMapping.Save();
			}
		}

		public void SaveYearCurrencyCourse(ClassifierViewModel model) {
			var u = new UnitOfWork();
			if (model.Id > 0) {
				var currency = u.Classifier.GetAll().FirstOrDefault(w => w.Id == model.Id);
				if (currency != null) {
						currency.Code = model.Course.ToString();
						u.Classifier.Edit(currency);
						u.Classifier.Save();
				}
			}
			else {
				var type = u.ClassifierType.GetAll().FirstOrDefault(w => w.Name == model.Type);
				var currency = u.Classifier.GetAll().FirstOrDefault(w => w.Id == model.CurrencyId);
				if (type == null || currency == null) return;
				var newClassif = new Classifier() { Name = $"Курс Евро на {model.Year} год ({currency.Name})", Code = model.Course.ToString(), Year = model.Year, Parent = currency, Active = true, ClassifierType = type, DateFrom = DateTime.Now, DateTo = DateTime.Now };
				u.Classifier.Add(newClassif);
				u.Classifier.Save();
			}
		}
	}
}