// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include "coursefile.h"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

static D3DBase one;
static bool bInit = false;

static HRESULT EnsureRendererManager()
{
	if(!bInit)
	{
		numtest = 1;
		camtype = 0; 
		numViews = 0;
		numriders = 0;
		large = true; 
		bInit = true;

	}
	return D3DBase::Create();
}
// Not used at this time
extern "C" HRESULT WINAPI SetSize(UINT &uWidth, UINT &uHeight)
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
	D3DBase::SetWindowSize(uWidth, uHeight);
Cleanup:
    return hr;
}

// Not used at this time
extern "C" HRESULT WINAPI InitRM1ExtDLL()
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
Cleanup:
    return hr;
}

// Not used at this time
extern "C" HRESULT WINAPI SetAlpha(BOOL fUseAlpha)
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
Cleanup:
    return hr;
}

// Not used at this time
extern "C" HRESULT WINAPI SetNumDesiredSamples(UINT uNumSamples)
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
Cleanup:
    return hr;
}

// Not used at this time
extern "C" HRESULT WINAPI SetAdapter(POINT screenSpacePoint)
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
Cleanup:
    return hr;
}

extern "C" HRESULT WINAPI GetBackBufferNoRef(void **ppSurface)
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
	return D3DBase::GetSurface(ppSurface);
Cleanup:
    return hr;
}

extern "C" HRESULT WINAPI Render()
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
	if(D3DBase::GetDevice())
	{
		//D3DBase::GetDevice()->getTimer()->tick();
		return D3DBase::Render();
	}
Cleanup:
    return hr;
}

extern "C" HRESULT WINAPI Render2()
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
	if(D3DBase::GetDevice())
	{
		//D3DBase::GetDevice()->getTimer()->tick();
		return D3DBase::Render2();
	}
Cleanup:
    return hr;
}

extern "C" HRESULT WINAPI LoadCourse(const char *coursename, int iscenery)
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
	if(D3DBase::GetDevice())
	{
		hr = D3DBase::LoadCourse(coursename, iscenery);
	}
Cleanup:
    return hr;
}

extern "C" HRESULT WINAPI SavePerf(int irider, const char *perfpath, PerformanceInfo *pperfi, PerfPoint **pperf, UINT coursetype)
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
	if(D3DBase::GetDevice())
	{
		hr = D3DBase::SavePerf(irider, perfpath, pperfi, pperf, coursetype);
	}
Cleanup:
    return hr;
}

extern "C" HRESULT WINAPI LoadCourseProgress(void **ptrresult)
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
	if(D3DBase::GetDevice())
	{
		hr = D3DBase::LoadCourseProgress(ptrresult);
	}
Cleanup:
    return hr;
}

extern "C" HRESULT WINAPI SendSystemCmds(const char *cmdstring, void **ptrresult)
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
	if(D3DBase::GetDevice())
	{
		if('_' == cmdstring[0] || '=' == cmdstring[0])
			hr = D3DBase::ParseGetSysCmds(cmdstring, ptrresult);
		else
			hr = D3DBase::ParseSysCmds(cmdstring);
	}
Cleanup:
    return hr;
}

extern "C" HRESULT WINAPI SendRiderCmds(int rider, const char *cmdstring, void **ptrresult)
{
    HRESULT hr = S_OK;
    IFC(EnsureRendererManager());
	if(D3DBase::GetDevice())
	{
		if('=' == cmdstring[0])
			hr = D3DBase::ParseGetRiderCmds(rider, cmdstring, ptrresult);
		else
			hr = D3DBase::ParseRiderCmds(rider, cmdstring);
	}
Cleanup:
    return hr;
}

extern "C" int WINAPI Get3DState()
{
	return (int)state;
}

extern "C" float WINAPI GetRiderDist(int irider)
{
	return (f32)D3DBase::rider[irider]->GetDist();
}

extern "C" void WINAPI Destroy()
{
	D3DBase::Destroy();
}

extern "C" void WINAPI SetBasePath(const char *basepath)
{
	strcpy(gBasePath,basepath);

    HRESULT hr = S_OK;
	// intialize here
//    IFC(EnsureRendererManager());

//Cleanup:
	HRESULT res = hr;
}
