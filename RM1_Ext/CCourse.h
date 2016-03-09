
#ifndef __C_COURSE_H_
#define __C_COURSE_H_

/***********************************************************************

***********************************************************************/
#include "StdAfx.h"
#include "d3dbase.h"
//#include "RiderData.h"

#if FALSE
class Logger  {

	private:
		char currentDirectory[_MAX_PATH];
		char file[64];
		char stub[32];
		BOOL closed;
		void cleanup_logfiles(void);
		Logger(const Logger&);						// unimplemented
		Logger &operator = (const Logger&);		// unimplemented

	public:

		Logger(const char *stub);
		virtual ~Logger(void);
		void write(int level, int printdepth, int reset, const char *format, ...);
		void write(const char *format, ...);
		void close(void);
		void dump(unsigned char *mb, int cnt);
		void flush(void);
		int loglevel;
		FILE *stream;
		
};
#else


#include <config.h>
//#include <logger.h>
#include <tlist.h>

#ifndef IGNOREFORNOW
class Tex;
class Model;
#endif
//#include <Tex.h>
#include <CourseFile.h>
//#include <Model.h>
//#include <TrackLoc.h>
#include <secfile.h>

#include <crf.h>
#include <vdefines.h>
//#define BIKE_APP

#ifdef VELOTRON
	#include <computrainerbase.h>
//	#include <crf.h>
#endif

#if defined(MULTI_APP) || defined(MULTIVID_APP)
//	#include "c:\\flo4\\multi\\rider.h"
#endif
#endif

#define MAX_GRADE			15.0f
#define MIN_GRADE			-15.0f
#define MAX_RIDERS		8
#define MAX_AVAIL_RIDERS		8

#pragma pack( push, 4 )

typedef struct _CourseVertex{
		core::vector3df v;
		union{
				DWORD diffuse;
				float color;
		};
		union{
				float tu1;
				void *pdata;
		};
		union{
				float tv1;    // texture coordinates
				int cel;
		};
} COURSEVERTEX;
#pragma pack( pop )

#define FVF_COURSEVERTEX	(D3DFVF_XYZ | D3DFVF_DIFFUSE | D3DFVF_TEX1)


#define CSECTION(a,b,c,d)	((a) | ((b)<<8) | ((c)<<16) | ((d)<<24))

#define COURSE_HEADER		CSECTION('C','T','3','D')



#define BIKE_HINTS	12
#define BIKE_EYE_HINT			(BIKE_HINTS-2)


#define MAX_TREES_PER_SECTION	10

#define MAX_STRIPS	10

class Terrain;


struct SectionInfo{
		float grade;
		float length;
		float deg;
		float startdist;
		float enddist;
		core::vector3df startloc;
		core::vector3df endloc;
		DWORD flags;

		float minlength;	// length this segment can be (in meters)
		float maxlength;

		float maxrotation;

		float elevation;
		float heading;

		float wind;
		//int n;						// the count of this section
};

#define SI_ADD			(1<<0)
#define SI_DELETE		(1<<1)
#define SI_ROTATE		(1<<2)
#define SI_LONGER		(1<<4)
#define SI_SHORTER		(1<<5)
#define SI_CLOSELOOP	(1<<6)
#define SI_FINISH		(1<<7)
#define SI_GRADE		(1<<8)
#define SI_WIND			(1<<9)


static char *type[8] = {							// must match enum Type
	"START",
	"FINISH",
	"STREIGHT",
	"CURVE",
	"CLOSELOOP",
	"EXTRA",
	"SPLIT",
	 "MAX_TYPE"
};

#ifndef IGNOREFORNOW
#ifdef BIKE_APP
	class CCourse : public D3DBase{
#else
	class CCourse  {
#endif
#else
	class CCourse  {
#endif
/*
	friend class willCourse;
//	friend class jimData;
	friend class Landscape;
	friend class BACourse;
	friend class CompuPerf;
	friend class BAEditor;
*/
	friend class Terrain;

	public:
		f32 GetMinElev() {return m_GridMin.y;}
		float get_totalMeters(void)  {
			return totalMeters;
		}
		float computeGrade(float _meters);
#ifdef BIKE_APP
		void analyze_perfs(void);
#endif

	private:
		unsigned long saved_file_size;
		//Logger *logg;
		bool message_shown;

		bool has_crs;
		int crs_nlegs;
		CRSLEG *crs;						// array of regularCourse legs
		float *accum_meters;				// array to hold the accumulated distances of regularCourse legs

		float minlen;
		float maxlen;
		float min_percent_grade;
		float max_percent_grade;
		float minangle;
		float maxangle;
		float minwind;
		float maxwind;
		float minElevation;
		float maxElevation;
		float maxElevGain;
		float totalMeters;
		long nlegs;
		bool closed;
		float avg_percent_grade;						// total average grade
		float avg_positive_percent_grade;			// average grade for grades above 0 only
		float avgwind;

	public:

		enum Textures {
			 TEX_ROAD,
			 TEX_EDGE_1_L,
			 TEX_EDGE_1_R,
			 TEX_EDGE_2_L,
			 TEX_EDGE_2_R,
			 TEX_EDGE_3_L,
			 TEX_EDGE_3_R,
			 TEX_EDGE_4_L,
			 TEX_EDGE_4_R,
			 TEX_EDGE_5_L,
			 TEX_EDGE_5_R,
			 TEX_START,
			 TEX_FINISH,
			 TEX_OVERRUN,
			 TEX_TREE1,							// 14
			 TEX_TREE1_SHADOW,
			 TEX_TREE2,							// 16
			 TEX_TREE2_SHADOW,
			 TEX_TREE3,							// 18
			 TEX_TREE3_SHADOW,
			 TEX_TREE4,							// 20
			 TEX_TREE4_SHADOW,
			 TEX_FAR_GROUND,
			 TEX_BACK_1,
			 TEX_BACK_2,
			 TEX_BACK_3,
			 TEX_BACK_4,
			 TEX_DOME,
			 TEX_MAX
		};

		double course_meters;

#ifndef IGNOREFORNOW
		class SignData : public TList<SignData>::Node{
			public:
				float dist;
				int seg;
				core::matrix4 mat;
				ITexture* ptex;
				//Model *pmodel;
				scene::IBillboardSceneNode* pmodel;
				bool center;
				SignData()  {
					SetData(this); ptex = NULL;
				}
				~SignData()  {
					//if (ptex)
					//	ptex->Release();
				}
		};
#endif
		CRF crf;


		//class Section : public TList<Section>::Node, scene::ISceneNode  {
		class Section : public TList<Section>::Node  {
//			friend class willCourse;

			public:
				static Section *ms_RenderList;
				enum EditMode {
					 NORMAL,
					 PROBLEM,
					 EDIT
				};
				enum Type {
					 START,
					 FINISH,
					 STREIGHT,
					 CURVE,
					 CLOSELOOP,		// Like a finish only different.
					 EXTRA,
					 SPLIT,
					 MAX_TYPE
				};

			private:
				int tree_group_index;			// which tree group this section is using
				int bp;
				static int n_sections;								// the sequence number of this section in a course

			protected:
				DWORD m_Flags;
				CCourse &m_Course;
				Type m_Type;
				float m_Length;		// section length In Meters.
				float grade_d_100;		// was m_Grade, Equivelent of the End grade.
				float m_DegRotation;		// In degrees.

				float m_Wind;			// In KM per hour.

				int m_SpecialTexture;
				int id;

				bool m_bLeftEdge, m_bRightEdge;

				// Calculated Values - use calcValues
				bool m_Valid;
				float m_StartDistance;// Start distance of this leg.
				int m_Divisions;	// Calculated based on the Grade/LastGrade, Rotation and Length.
				float m_DivisionLength;
				float start_grade_d_100;
				float m_StartWind;
				core::vector3df m_StartLoc;		
				core::vector3df m_EndLoc;

				core::vector3df m_Center;	// For rotation
				float m_Radius;
				float m_Rotation;
				float m_StartRotation;
				float m_RotationStep;
				float m_EndRotation;

				float m_StartTV;		// So we don't have roundoff error in the texture wraping.

				float *m_YArr;	// Height's at each divisons.
				float *m_GArr;	// Grade at each division

				float m_LowY;
				float m_HighY;

				float m_ActualLength;

				// Extents
				core::vector3df m_ExtOrigin;
				float m_ExtRadius;
				core::vector3df m_ExtMin;
				core::vector3df m_ExtMax;

				// Model stuff
				//scene::CVertexBuffer m_pVBuf;
				//scene::SMeshBuffer *m_pMeshBuf;
				//scene::SMesh *m_pMeshBuf;
				
				//scene::SMesh *m_pMesh1;
				//scene::SMesh *m_pMesh2;
				
				CCustomSceneNode* m_pSectionNode;
				bool bMeshCreated;

				LPWORD m_pIBuf;
				DWORD m_VertCount;
				DWORD m_IndexCount;

				enum RenderIns {
					 RINS_END,		//					End the model
					 RINS_TEX,		// <num>			Sets the texture
					 RINS_STRIP,		// <minvert> <vertcnt> <startidx> <cnt>
					 RINS_STRIP_T,	// <startvert> tree strip
					 RINS_TRI,		// 
					 RINS_TREESTART,
					 RINS_TREEEND
				};
				short *m_pRenderIns;
				void cmd_addStrip(short *&pcmd, int sv, int ev, int sidx, int eidx, WORD *idx);
				void cmd_addTri(short *&pcmd, int sv, int ev, int sidx, int eidx, WORD *idx);
				void cmd_addTex(short *&pcmd, int tex);
				void reStrip(COURSEVERTEX *pv, COURSEVERTEX *pvs, int cnt, float tu, float repeat);

				// Copy forward from last data.
				struct StripData{
						float color;
						int count;
						float ydelta;
						float addy;
						int updown;
						int n;
				};
				float m_StartLeft;
				float m_StartRight;
				float m_EndLeft;
				float m_EndRight;
				COURSEVERTEX m_EndV[MAX_STRIPS];
				StripData m_EndStripData[MAX_STRIPS];
				bool m_EndStripValid;
				void bez(int addindex, StripData &sd, COURSEVERTEX *cv);

				// TREES 
				int m_TreeCount;
				int m_LastTreePattern;
				IBillboardSceneNode **m_pTreeArr;


#ifndef IGNOREFORNOW
				// Sign
				TList<SignData> m_SignList;
#endif
				int m_FrameNum;

				// Terrain
				TList<Terrain*> m_pTerrainList;


				// Edit Model stuff
				//scene::CVertexBuffer m_pVBufEdit;
				int m_RoadPoints;
				int m_TopPoints;

				video::SColor m_RoadColor;
				video::SColor m_TopColor;

				core::vector2df *m_pCArr;
				void calcCArr();
				void closeCArr();

				EditMode m_EditMode;


				float m_LowEdge;	// How much do we need to go down on the edges to make sure that there is no "Under view"

				int m_NonDraw;

				void extAt(float dist);
				void init(Type type, float add, float grade, float rot, int special = TEX_ROAD,
							 float startdist = 0.0f);

				void initYArr();

			public:
				Section(CCourse &course, Type type, float add, float grade, float rot, int special=TEX_ROAD, float startdist=0.0f);
				Section(CCourse &course, class SecFile &sf);
				~Section();

				//tlm+++
				bool SetLength(float _meters);
				bool SetAngle(float _angle);
				void dump(int totalCount, int regularCount, FILE *stream, bool _metric, bool showMiles);
				//tlm---

				int GetDivisions()		{ return m_Divisions; }

				bool Save(class SecFileWrite &sf);

				bool ReadyModel(bool doends = false);

				#ifdef BIKE_APP		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
					// Section::
					/*
					core::aabbox3d<f32> Box;
					video::SMaterial Material;
					virtual void OnAnimate(u32 timeMs){}
					virtual void OnRegisterSceneNode();
					virtual const core::aabbox3d<f32>& getBoundingBox() const
					{
						return Box;
					}

					virtual u32 getMaterialCount() const
					{
						return 1;
					}

					virtual video::SMaterial& getMaterial(u32 i)
					{
						return Material;
					}	
					*/
					virtual void render();

					bool ReadyEditModel();
					void CloseModel();
					//HRESULT Render();
					void RenderEditRoad();
					void RenderEditTop();
					void Show(bool bShow);
					//bool AddSign(const char *texname, float dist, IBillboardSceneNode *pmodel, bool center);
				#endif

				CCustomSceneNode* GetSectionNode() { return m_pSectionNode; }

				Section *m_pNextRender;
				bool m_bRender;

				void AddRender()  {
					//if (!m_bRender)  {
						m_pNextRender = ms_RenderList;
						ms_RenderList = this;
						m_bRender = true;
					//}
				}
				static void ClearRenderList()  {
					ms_RenderList = NULL;
				}
				static void ClearRendered()  {
					Section *ptNext;
					for(Section *pt = ms_RenderList;pt;pt = ptNext)
					{
						ptNext = pt->m_pNextRender;
						pt->Show(false);
						pt->m_pNextRender = NULL;
						pt->m_bRender = false;
					}
					ms_RenderList = NULL;
				}

				core::vector3df GetEndLoc() const  {
					return m_EndLoc;
				}
				float GetEndRotation() const  {
					return m_EndRotation;
				}
				int getType(void)  {
					return m_Type;
				}
				float GetAngle(void) const { 
					return m_DegRotation;		// In degrees.
				}

				float GetEndDistance() const  {
					return m_StartDistance + m_Length;
				}
				const core::vector3df & GetExtOrigin() const  {
					return m_ExtOrigin;
				}
				float GetExtRadius() const  {
					return m_ExtRadius;
				}

				float GetSectionRotation() const  {
					return m_Rotation;
				}

				core::vector3df GetStartLoc() const  {
					return m_StartLoc;
				}
				float GetStartDistance() const  {
					return m_StartDistance;							// distance in meters
				}
				float GetLength() const  {							// length of a Section!
					return m_Length;
				}

				float GetActualLength()	const  {
					return (m_ActualLength > 0.0f ? m_ActualLength : m_Length);
				}

				float GetDegRotation() const  {
					return m_DegRotation;
				}
				float GetRotation() const  {
					return m_Rotation;
				}

				float get_grade_d_100() const  {
					return grade_d_100;
				}


				float GetWind()	const  {
					return m_Wind;							// this is metric!
				}

				bool RoadLoc(
						Section **ppsec, 
						float dist, 
						core::vector3df &loc, 
						core::vector3df &vec,
						float &grade, 
						float *pangle = NULL, 
						float *pwind = NULL);

				bool IsFinishSection() const  {
					return (m_Type == FINISH);
				}



				void SetEditMode(EditMode em = NORMAL);
				EditMode GetEditMode()const  {
					return m_EditMode;
				}

				bool Set(float deg, float len);
				bool set_grade_d_100(float grade);
				bool SetWind(float wind);

				bool NoRender();

				void ClearAllSigns();

				float CalcEndHeight(float endgrade);
				float CalcEndGrade(float ey);

				friend CCourse;
			private:
				void calcValues();
		};						// class Section


	protected:
		char *m_pName;			// Base name -- no extention.
		char *m_pDescription;		// Optional description
		char *m_pFileName;		// Last Saved file name.	(Including lastprefs.
		char *m_pCreator;			// Creator of the file
		char m_Error[256];

		int m_Version;
		bool m_Finished;
		float m_Length;						// CCourse length in meters

		int m_TotalLaps;

		video::SMaterial Material;
		char *m_Tex[TEX_MAX];
		//Tex *m_Tex[TEX_MAX];

#ifndef IGNOREFORNOW

		IBillboardSceneNode *m_pSign;
		IBillboardSceneNode *m_pBanner;
#endif
		Section *GetPrevSection(Section *ps);	// Takes into account looping tracks.

		PerformanceInfo pinf[MAX_RIDERS];
		struct PerfPoint *m_pPerfArr[MAX_RIDERS];


#ifdef VELOTRON
#ifdef LOG_GEARING
		ComputrainerBase::GEARINFO *gearInfoArray[MAX_RIDERS];
#endif
#endif

#ifdef LOG_CALORIES
		float *calories[MAX_RIDERS];
#endif

		TList<Section> m_Track;

		Section *m_pLocCache[BIKE_HINTS];

		friend Section;

		// The master grid.
		int m_GridWidth, m_GridHeight;
		core::vector3df m_GridMin, m_GridMax;
		core::vector3df m_GridSquareSize;
		float *m_GridYArr;
		//void calcGrid(float size = 50.0f);	// Calculates the extents of the grid.
		//void calcTerrainGrid(float size = 200.0f, float extend = 200.0f);
		void calcGrid(float size = 100.0f);	// Calculates the extents of the grid.
		void calcTerrainGrid(float size = 300.0f, float extend = 300.0f);

		void calcTan();

		float m_LowY, m_HighY;

		bool m_bLoopClosed;
		bool m_bLooped;
		bool m_bMirror;
		bool m_bReverse;
		float m_StartAt;
		float m_EndAt;

		// Edge Mesh
		bool m_bBuildingEdgeMesh;
		//scene::SMesh *m_pMesh;
		ISceneNode* m_pSceneNode;
		ISceneNode* m_pNodeVisible;
		ISceneNode* m_pNodeHidden;

#ifndef IGNOREFORNOW
//		class GLModelDef *m_pMeshDef;
//		class GLModel *m_pMesh;
#endif
		Section *m_pStartSection;		// Must be a START piece.
		Section *m_pFinishSection;		// Can be any type - on loop tracks it will be the 
		// piece after the start.

		Section *m_pFinish;				// Set only by the Section new when a finish piece is detected.

		float m_CourseLength;


		int m_FrameNum;
		bool m_Dirty;				// Need to rebuild the terrain.

		bool loadRMP(char *fname);
		bool loadOld(FILE *file);
		bool loadNew(FILE *file);
		bool is_encrypted(char *fname);


		// Edit stuff
		SectionInfo m_SI;
		Section *m_pSISection;
		Section *m_pEdit;
		SectionInfo &updateSI(bool ignoreColl = false);

		int laps;

		float m_EyeDistance;
		float m_BikeDistance;
		int m_EyeNum;
		float m_FarDistance;

		bool m_FogOn;
		video::SColor m_FogColor;
		float m_FogDensity;


		float m_SecondStart;

		int m_SectionCount;

		core::vector3df m_Offset;
		float m_BaseRotation;



		bool addEndSegments();  
		bool undo(Section *ps);

		void finishTrack();
		Section *splitSection(Section *ps, float splitlen);	// Splits this and returns the new section (placed before the ps)
		float doGrade(Section *pstart, Section *pend, float grade);

		// Terrain
		COURSEVERTEX *m_pEdge;
		int m_EdgeTotal;
		int m_EdgeCount[2];

		TList<Terrain> m_TerrainList;
		Terrain **m_pTerrainGrid;
		void addTerrain(COURSEVERTEX *pedge, int count, int start, int leftstart);

		// Bar
		float m_BarTotal;
		float m_BarCount;
		float m_BarNext;
		float m_BarMax;

	private:
		char fullpath[256];
		char curpath[256];
		bool metric;
		int bp;
		void cleanup(void);
		int sky_r;
		int sky_g;
		int sky_b;
		float sky_tiled;
		float ms_usize;							// 20.0f, smaller = more repeats, passed to Terrain::
		float ms_vsize;							// 20.0f, smaller = more repeats, passed to Terrain::
		char padding[256];

		CCourse(const CCourse&);						// unimplemented	
		CCourse &operator = (const CCourse&);		// unimplemented
		void doo(const char *_fname);							// obfuscated "encrypt()"
		void unencrypt(const char *_fname);


	public:
		CCourse(bool _metric=false, bool no3d = false, const char *scenery = "standard");
		virtual ~CCourse();
		/*
		void set_logger(Logger *_logg)  {
			logg = _logg;
			return;
		}
		*/

		void barAdd(int num = 1, const char *message="Building Track");
		void barInit(int num, float max);
		void barRescale(int num, float max);
		float barProgress() {return m_BarCount / m_BarTotal;}

		char *getTex(int idx) {return m_Tex[idx];}
		SColor getSkyColor() {return SColor(255, sky_r, sky_g, sky_b);}
		float getSkyDomeTiled() {return sky_tiled;}

		//tlm+++
		int GetVersion(void);
		void dump(char *fname=NULL, bool showMiles=false);
		int countSections(void);								// returns the number of "regular" sections
		Section * GetFirstSection(void)  {
			return m_Track.GetFirst();
		}
		/*********************************************************************************

		*********************************************************************************/

		Section * GetFirstRealSection(void)  {
			Section *ps = NULL;

			ps = m_Track.GetFirst();
			for(;ps && (ps->GetStartDistance() < 0.0f);)  {		// <0 means that we're on the course "lead in"
				ps = ps->Next();
			}
			return ps;
		}


		const char * GetCourseName()  {
			return (m_pName ? m_pName : "???");
		}
		const char * GetCourseDescription()  {
			return (m_pDescription ? m_pDescription : "");
		}
		const char * GetCourseCreator()  {
			return (m_pCreator ? m_pCreator : "Unknown");
		}
		const char * GetCourseFileName()  {
			return (m_pFileName ? m_pFileName : "???");
		}
		const char *get_full_path(void)  {
			return fullpath;
		}

		void SetCourseName(const char *name);
		void SetCourseDescription(const char *desc);
		void SetCourseCreator(const char *desc);

		bool Load(const char *name);
		bool LoadAny(const char *name);

		#if defined(MULTI_APP) || defined(MULTIVID_APP)
		//bool make(Rider *_r);
		#endif

		bool Save(const char *name, int num = -1);
		bool SavePerf(int num, const char *basename, PerformanceInfo *pperfi, PerfPoint *perfs, UINT coursetype);

		Section * RoadLocOld(
					float dist, 
					core::vector3df &loc, 
					core::vector3df &vec, 
					float &grade, 
					int bikehint = 0,
					float *pangle = NULL, 
					float *dangle = NULL, 
					float *pwind = NULL,
					Section *psec = NULL
					);
		Section * RoadLoc(
					float dist, 
					core::vector3df &loc, 
					core::vector3df &vec, 
					float &grade, 
					int bikehint = 0,
					float *pangle = NULL, 
					float *dangle = NULL, 
					float *pwind = NULL,
					Section *psec = NULL,
					float *endoftrack = NULL
					);


		float Rotation(float dist);

		virtual HRESULT Open();			// Called when a d3d device goes online.
		virtual HRESULT Restore();		// Called when a device is restored.
		virtual HRESULT Invalidate();	// Called when a device goes down.
		virtual HRESULT Close();		// Called when a device gets destroyed.


		float GetEndOfTrack() const;
		float CourseDistance() const;
		void RedoCourse();
		float GetCourseLength() const;
		float GetRaceLength() const;
		int GetLapNum(float len) const;
		float GetLapPos(float dist) const;		// Return a number from 0 to 1 where it is in the lap.


		bool BuildTerrain(bool displaybar = true);

		void ClearTrack();

		void ClearProfile(int num = -1);
		bool IsRaceable();	// Check the course and see if it is ready.
		bool IsSaveable();

		bool IsPrefSaveable(int num = -1);

		int GetPerfPointCount(int num) const  {
			return pinf[num].perfcount;
		}

		PerformanceInfo *GetPerfInfo(int num)  {
			return &pinf[num];
		}

		PerfPoint * GetPerfPointArray(int num) const  {
			return m_pPerfArr[num];
		}

		virtual void computeStats(void);

		#ifdef VELOTRON
		#ifdef LOG_GEARING
		ComputrainerBase::GEARINFO * GetGearInfoArray(int num) const  {
			return gearInfoArray[num];
		}
		#endif
		#endif

		#ifdef LOG_CALORIES
		float *GetCaloriesArray(int num) const  {
			return calories[num];
		}
		#endif

		#ifdef BIKE_APP
			// CCourse:: (public)
			virtual HRESULT Update(int advtime);
			virtual HRESULT Render();
			virtual void HideAll();
			virtual void ShowVisible(ICameraSceneNode* cam, Section *psec, bool bShow=true);

			bool save_performance(int num, class ComputrainerBase &ct);

#ifndef IGNOREFORNOW
			bool GetPerfRiderData(int num, struct RiderData &rd);
#endif
			void ClearTerrain();
			bool SetLandscape(const char *name);
			bool SetLandscape2(const char *name);
			void SetCourseFog();
		#endif


		const char *IsLater(const char *pn0, const char *pn1);

		// Grid varables
		const core::vector3df & GetGridMin() const  {
			return m_GridMin;
		}
		const core::vector3df & GetGridMax() const  {
			return m_GridMax;
		}
		int GetGridWidth() const  {
			return m_GridWidth;
		}
		int GetGridHeight() const  {
			return m_GridHeight;
		}
		float GetGridSquareWidth() const  {
			return m_GridSquareSize.x;
		}
		float GetGridSquareHeight() const  {
			return m_GridSquareSize.z;
		}

		void ReadyRace();	// Ready the track for racing.
		// EDITING
		void ReadyEdit();	// Ready the track for editing.
		const SectionInfo &GetSectionInfo(bool force = false);
		const SectionInfo &EditDeg(float deg);
		const SectionInfo &EditLength(float len);
		const SectionInfo &EditGrade(float grade);
		const SectionInfo &EditWind(float wind);
		bool EditAdd(int dir);
		bool EditDelete();
		bool EditNormalize();
		bool EditFinish();
		bool EditCloseLoop(bool create = true, bool force = false);
		bool EditRandom(float len, float grade, float wind);
		bool FixCloseLoop(bool create = true, bool force = false);


		bool CheckCollision(Section *ps, Section *pc);
		bool CheckCollisionLine(Section *ps, float x1, float y1, float x2, float y2);

		void SetLaps(int laps);
		int getLaps(void) const  {
			return (m_bLoopClosed ? laps : 1);
		}

		bool IsLoopClosed() const  {
			return m_bLoopClosed;
		}
		bool IsLooped() const  {
			return m_bLooped;
		}
		bool IsMirrored() const  {
			return m_bMirror;
		}
		bool IsReversed() const  {
			return m_bReverse;
		}
		f32 StartAt() const  {
			return m_StartAt;
		}
		f32 EndAt() const  {
			return m_EndAt;
		}

		HRESULT RenderEdit();

#if defined(BIKE_APP) || defined(MULTI_APP) || defined(MULTIVID_APP) || defined(TOPO_APP)
		bool myImport(const char *name);
#endif

		bool Export(const char *name);

		//Tex * GetTexture(Textures texnum)  {
		//	return m_Tex[texnum];
		//}

		void SetEyeDistance(float dist, float bdist, int num)  {
			m_EyeDistance = dist;m_BikeDistance = bdist, m_EyeNum = num;
		}

		float GetFarDistance()  {
			return m_FarDistance;
		}

		float AdjustRiderDistance(float dist);

		float GetTotalTrack()  {
			return m_Track.GetLast()->GetEndDistance();
		}

		//bool AddSign(const char *texname, float dist, IBillboardSceneNode *pmodel, bool center);
		void ClearAllSigns();

		float GetSecondStart()  {
			return m_SecondStart;
		}
		void SetSecondStart(float s)  {
			m_SecondStart = s;
		}

		bool ReadyModel();
		void ClearModels();

		ISceneNode* GetCourseNode() {return m_pSceneNode;}
		void SetCourseNode(ISceneNode* pSceneNode) {m_pSceneNode = pSceneNode;}

		//ISceneNode* GetCourseNodeHidden() {return m_pSceneNodeHidden;}
		//void SetCourseNodeHidden(ISceneNode* pSceneNodeHidden) {m_pSceneNodeHidden = pSceneNodeHidden;}

		void GridXY(float xx, float zz, int &x, int &z)  {
			x = (int) ((xx - m_GridMin.x) / m_GridSquareSize.x);
			z = (int) ((zz - m_GridMin.z) / m_GridSquareSize.z);
			if (x < 0)
				x = 0;
			else if (x >= m_GridWidth)
				x = m_GridWidth - 1;
			if (z < 0)
				z = 0;
			else if (z >= m_GridHeight)
				z = m_GridHeight - 1;
		}
		int GridCel(float xx, float zz)  {
			int x, z;
			GridXY(xx, zz, x, z);
			return x + z * m_GridWidth;
		}

		/*****************************************************************

		*****************************************************************/

		int Renumber()  {
			Section *ps;
			int i;

			for (ps = m_Track.GetFirst(),i = 0; ps; ps = ps->Next(),i++)  {
				ps->id = i;
			}

			m_SectionCount = i;
			return i;
		}

		/*****************************************************************

		*****************************************************************/

		void SidesOff()  {
			Section *ps;
			for (ps = m_Track.GetFirst(); ps; ps = ps->Next())  {
				#ifdef BIKE_APP
					ps->CloseModel();
				#endif
				ps->closeCArr();
				ps->m_bLeftEdge = ps->m_bRightEdge = false;
			}
		}

		bool TurnOffABSSectionEnds();		// Turns of the aboslulte - section ends for the track.  NOTE: WILL RETURN FALSE IF THE TRACK IS LOOPED!


		#ifndef WEBAPP
			bool ImportCSV(const char *name);
			//bool Make(TrackLoc &track);
		#endif

		int make_crs(const char *_outfname);

		#if defined(MULTI_APP) || defined(TOPO_APP) || defined(VIDEO_APP)
			bool Make(TrackLoc &track);
		#endif

};


#define SECTION_F_ABS_STARTLOC		(1<<0)	// Start location of this section is not based on the end -position of the last segment.
#define SECTION_F_ABS_Y				(1<<1)	// Y values are lock and so is the number of divisions.


#endif		//#ifndef _COURSE_H_

