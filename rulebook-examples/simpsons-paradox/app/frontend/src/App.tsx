import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Shell } from './components/Shell';
import { OverviewView } from './views/OverviewView';
import { StratumView } from './views/StratumView';
import { WeightsView } from './views/WeightsView';
import { SandboxView } from './views/SandboxView';
import { ModelSummaryView } from './views/ModelSummaryView';

export function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Shell />}>
          <Route index element={<Navigate to="/overview" replace />} />
          <Route path="overview" element={<OverviewView />} />
          <Route path="stratum" element={<StratumView />} />
          <Route path="weights" element={<WeightsView />} />
          <Route path="sandbox" element={<SandboxView />} />
          <Route path="model" element={<ModelSummaryView />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
