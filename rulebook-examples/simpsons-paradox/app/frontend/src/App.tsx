import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Shell } from './components/Shell';
import { OverviewView } from './views/OverviewView';
import { StratumView } from './views/StratumView';
import { WeightsView } from './views/WeightsView';
import { SandboxView } from './views/SandboxView';
import { ModelSummaryView } from './views/ModelSummaryView';
import { AllocationSweepView } from './views/AllocationSweepView';
import { PhaseDiagramView } from './views/PhaseDiagramView';
import { ImportCatalogView } from './views/ImportCatalogView';
import { InstrumentDashboardView } from './views/InstrumentDashboardView';
import { ConclusionsAdminView } from './views/ConclusionsAdminView';
import {
  DagFieldPage,
  DagIndexPage,
  DagTablePage,
  ExplainerEnhance,
  useExplainerRouting,
} from './explainer-bridge';

function AppRoutes() {
  useExplainerRouting();
  return (
    <>
      <ExplainerEnhance />
      <Routes>
        <Route path="/" element={<Shell />}>
          <Route index element={<Navigate to="/overview" replace />} />
          <Route path="conclusions" element={<ConclusionsAdminView />} />
          <Route path="overview" element={<OverviewView />} />
          <Route path="stratum" element={<StratumView />} />
          <Route path="weights" element={<WeightsView />} />
          <Route path="sandbox" element={<SandboxView />} />
          <Route path="model" element={<ModelSummaryView />} />
          <Route path="sweep" element={<AllocationSweepView />} />
          <Route path="phase" element={<PhaseDiagramView />} />
          <Route path="instrument" element={<InstrumentDashboardView />} />
          <Route path="catalog" element={<ImportCatalogView />} />
        </Route>
        <Route path="/dag" element={<DagIndexPage />} />
        <Route path="/dag/:table" element={<DagTablePage />} />
        <Route path="/dag/:table/:field" element={<DagFieldPage />} />
      </Routes>
    </>
  );
}

export function App() {
  return (
    <BrowserRouter>
      <AppRoutes />
    </BrowserRouter>
  );
}
