using Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Person: Model, IDynamicTree {

	public string FullName {
		get {
			return this.FirstName + " " + this.SecondName;
		}
		set {
			string[] names = value.Split(' ');
			this.FirstName = names[0];
			this.SecondName = names[1];
		}
	}

	public int? Id;

	public string FirstName { get; set; }

	public string SecondName { get; set; }

	public string Sex { get; set; }

	public int? BossId;

	public bool HasChilds = false;
		
	public static Person GetById (int id) {
		SqlDataReader r = Command.Prepare(
			"SELECT * FROM dbo.Person AS p WHERE p.Id = @param"
		).Query(new { param = id });
		return Model.SetUp<Person>(r);
	}

	public List<IDynamicTree> GetByParentId (string parentId) {
		SqlDataReader r;
		string sql = @"
			SELECT 
				p.*,
				IIF(ISNULL(counts.Cnt, 0) > 0, 1, 0) AS HasChilds
			FROM
				dbo.Person AS p
			LEFT JOIN (
				SELECT 
					p.IdBoss,
					COUNT(p.Id) AS Cnt
				FROM
					dbo.Person AS p
				WHERE
					p.IdBoss IS NOT NULL AND
					p.IdBoss IN (
						SELECT 
							Id
						FROM
							dbo.Person AS p
						WHERE 
							{0}
					)
				GROUP BY
					p.IdBoss
			) AS counts ON
				counts.IdBoss = p.Id 
			WHERE 
				{1}
		";
		if (parentId == "") {
			sql = String.Format(sql, "p.IdBoss IS NULL", "p.IdBoss IS NULL");
			r = Command.Prepare(sql).Query();
		} else {
			sql = String.Format(sql, "p.IdBoss = @param1", "p.IdBoss = @param2");
			r = Command.Prepare(sql).Query(new { param1 = parentId, param2 = parentId });
		}

		List<Person> persons = Model.SetUpToList<Person>(r);
		List<IDynamicTree> result = new List<IDynamicTree>();
		//persons.ForEach(p => result.Add(p));
		persons.ForEach((person) => {
			result.Add(person);
		});
		return result;
	}

	public bool GetHasChilds () {
		return this.HasChilds;
	}

	public string GetUId () {
		return this.Id.ToString();
	}

	public bool Remove () {
		// odstraní z databáz podle this.Id

		return true;
	}


	public bool Save () {
		// updatuje datbazi podle this.Id
		// vse co bylo zmeneno je v this.touchedStore;
		return true;
	}
}