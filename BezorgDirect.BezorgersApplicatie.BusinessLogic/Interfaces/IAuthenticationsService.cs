using System;
using System.Collections.Generic;
using System.Text;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces
{
    /* This interface was setup by Lennart de Waart (563079) */
    public interface IAuthenticationsService // Contract for AuthenticationsService, service may be changed
    {
        string getTokenFromHeader(string authHeader);
        bool IsTokenValid(string token, bool isAdminToken);
    }
}
