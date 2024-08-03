using Microsoft.AspNetCore.Mvc;
using PFDB.SQLite;
using PFDB.WeaponStructure;
using PFDB.WeaponUtility;

namespace Frontend.Server.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PFDBController : Controller
	{
		public PFDBController()
		{
		}

		[HttpGet(Name = "GetPFDB")]
		public PhantomForcesDataModel Get()
		{
			return new PhantomForcesDataModel(PhantomForcesDataModel.GetWeaponCollection(new PhantomForcesVersion("9.0.2")));
		}
	}
}
