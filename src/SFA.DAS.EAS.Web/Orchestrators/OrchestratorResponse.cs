﻿using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Orchestrators;

public class OrchestratorResponse
{
    public OrchestratorResponse()
    {
        Status = HttpStatusCode.OK;
    }
    public HttpStatusCode Status { get; set; }
    public Exception Exception { get; set; }
    public FlashMessageViewModel FlashMessage { get; set; }

    public string RedirectUrl { get; set; }
    public string RedirectButtonMessage { get; set; }
}

public class OrchestratorResponse<T> : OrchestratorResponse
{
    public T Data { get; set; }
}