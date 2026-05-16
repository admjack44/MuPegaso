import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import styles from './SelectServer.module.css';

/**
 * @typedef {'available'|'full'|'saturated'|'maintenance'} ServerStatus
 * @typedef {{ id: string, name: string, num: number, groupId: string, status: ServerStatus, isNew?: boolean }} Server
 * @typedef {{ onConnect: (serverName: string) => void, onClose?: () => void, className?: string }} SelectServerProps
 */

const GROUPS = [
  { key: 'recent', label: 'Recientes' },
  { key: 'SV1', label: 'Grupo SV1' },
  { key: 'SV2', label: 'Grupo SV2' },
  { key: 'SV3', label: 'Grupo SV3' },
];

/** Últimos visitados (orden: más reciente primero) */
const RECENT_SERVER_NUMS = [21, 7];

function buildGroupServers(groupId, start, end) {
  const total = end - start + 1;
  const rest = total - 1;
  const nFull = Math.floor(rest * 0.6);
  const nSat = Math.floor(rest * 0.2);
  const nMaint = rest - nFull - nSat;
  const pool = [
    ...Array(nFull).fill('full'),
    ...Array(nSat).fill('saturated'),
    ...Array(nMaint).fill('maintenance'),
  ];
  let pi = 0;
  const out = [];
  for (let num = start; num <= end; num++) {
    if (num === end) {
      out.push({
        id: `${groupId}-${num}`,
        name: `MU PEGASO ${num}`,
        num,
        groupId,
        status: 'available',
        isNew: true,
      });
    } else {
      out.push({
        id: `${groupId}-${num}`,
        name: `MU PEGASO ${num}`,
        num,
        groupId,
        status: pool[pi++] || 'full',
        isNew: false,
      });
    }
  }
  return out;
}

function buildAllServers() {
  return [
    ...buildGroupServers('SV1', 1, 13),
    ...buildGroupServers('SV2', 14, 22),
    ...buildGroupServers('SV3', 23, 30),
  ];
}

const ALL_SERVERS = buildAllServers();

function getServersForGroup(groupKey) {
  if (groupKey === 'recent') {
    return RECENT_SERVER_NUMS.map((num) =>
      ALL_SERVERS.find((s) => s.num === num),
    ).filter(Boolean);
  }
  return ALL_SERVERS.filter((s) => s.groupId === groupKey);
}

const STATUS_LABEL = {
  available: 'Disponible',
  full: 'Lleno',
  saturated: 'Saturado',
  maintenance: 'Mantenimiento',
};

function HexIcon({ className }) {
  return (
    <svg
      className={className}
      viewBox="0 0 32 32"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      aria-hidden
    >
      <path
        d="M16 2L27 9V23L16 30L5 23V9L16 2Z"
        stroke="currentColor"
        strokeWidth="1.5"
        fill="rgba(37,99,235,0.12)"
      />
      <path
        d="M16 8L22 12V20L16 24L10 20V12L16 8Z"
        stroke="currentColor"
        strokeWidth="1"
        fill="rgba(59,130,246,0.15)"
      />
    </svg>
  );
}

function ParticlesCanvas() {
  const ref = useRef(null);

  useEffect(() => {
    const canvas = ref.current;
    if (!canvas) return undefined;
    const ctx = canvas.getContext('2d');
    if (!ctx) return undefined;

    const N = 18;
    const parent = canvas.parentElement;
    let raf = 0;
    const particles = [];

    const resize = () => {
      if (!parent) return;
      const w = parent.clientWidth;
      const h = parent.clientHeight;
      const dpr = Math.min(window.devicePixelRatio || 1, 2);
      canvas.width = w * dpr;
      canvas.height = h * dpr;
      canvas.style.width = `${w}px`;
      canvas.style.height = `${h}px`;
      ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
    };

    const initParticles = () => {
      particles.length = 0;
      const w = canvas.clientWidth || 300;
      const h = canvas.clientHeight || 400;
      for (let i = 0; i < N; i++) {
        particles.push({
          x: Math.random() * w,
          y: Math.random() * h,
          r: 1 + Math.random() * 2,
          vy: 0.15 + Math.random() * 0.45,
        });
      }
    };

    const tick = () => {
      const w = canvas.clientWidth;
      const h = canvas.clientHeight;
      ctx.clearRect(0, 0, w, h);
      ctx.fillStyle = 'rgba(96,165,250,0.3)';
      for (const p of particles) {
        p.y -= p.vy;
        if (p.y < -4) {
          p.y = h + 4 + Math.random() * 40;
          p.x = Math.random() * w;
        }
        ctx.beginPath();
        ctx.arc(p.x, p.y, p.r, 0, Math.PI * 2);
        ctx.fill();
      }
      raf = requestAnimationFrame(tick);
    };

    resize();
    initParticles();
    const ro = new ResizeObserver(() => {
      resize();
      initParticles();
    });
    ro.observe(parent);
    raf = requestAnimationFrame(tick);

    return () => {
      cancelAnimationFrame(raf);
      ro.disconnect();
    };
  }, []);

  return <canvas ref={ref} className={styles.particles} aria-hidden />;
}

/**
 * @param {SelectServerProps} props
 */
export default function SelectServer({ onConnect, onClose, className }) {
  const [panelMounted, setPanelMounted] = useState(false);
  const [activeGroup, setActiveGroup] = useState('SV1');
  const [displayedGroup, setDisplayedGroup] = useState('SV1');
  const [gridAnim, setGridAnim] = useState('shown'); // shown | hiding | entering
  const [selectedId, setSelectedId] = useState(null);
  const hideTimer = useRef(null);
  const enterTimer = useRef(null);

  useEffect(() => {
    const id = requestAnimationFrame(() => setPanelMounted(true));
    return () => cancelAnimationFrame(id);
  }, []);

  const selectGroup = useCallback((key) => {
    if (gridAnim !== 'shown') return;
    if (key === displayedGroup) return;

    setSelectedId(null);
    setActiveGroup(key);
    setGridAnim('hiding');

    if (hideTimer.current) clearTimeout(hideTimer.current);
    if (enterTimer.current) clearTimeout(enterTimer.current);

    hideTimer.current = setTimeout(() => {
      setDisplayedGroup(key);
      setGridAnim('entering');
      enterTimer.current = setTimeout(() => {
        setGridAnim('shown');
      }, 200);
    }, 150);
  }, [displayedGroup, gridAnim]);

  useEffect(
    () => () => {
      if (hideTimer.current) clearTimeout(hideTimer.current);
      if (enterTimer.current) clearTimeout(enterTimer.current);
    },
    [],
  );

  const gridServers = useMemo(
    () => getServersForGroup(displayedGroup),
    [displayedGroup],
  );

  const selected = useMemo(
    () => gridServers.find((s) => s.id === selectedId) ?? null,
    [gridServers, selectedId],
  );

  const onCardClick = (server) => {
    if (server.status === 'maintenance') return;
    setSelectedId((cur) => (cur === server.id ? null : server.id));
  };

  const gridClass =
    gridAnim === 'hiding'
      ? styles.gridHiding
      : gridAnim === 'entering'
        ? styles.gridEntering
        : styles.gridShown;

  return (
    <div
      className={[styles.root, className || ''].filter(Boolean).join(' ')}
    >
      <ParticlesCanvas />
      <div className={styles.shell}>
        <div
          className={`${styles.panel} ${panelMounted ? styles.panelMounted : ''}`}
        >
          <div className={styles.accentBar} />
          <div className={styles.scanline} />
          <span className={`${styles.corner} ${styles.cornerTL}`} />
          <span className={`${styles.corner} ${styles.cornerTR}`} />
          <span className={`${styles.corner} ${styles.cornerBL}`} />
          <span className={`${styles.corner} ${styles.cornerBR}`} />

          <header className={styles.header}>
            <div className={styles.headerLeft}>
              <HexIcon className={styles.hexIcon} />
              <div className={styles.titleBlock}>
                <div className={styles.title}>SELECCIONAR SERVIDOR</div>
                <div className={styles.subtitle}>
                  MU PEGASO · Elige tu reino y conéctate
                </div>
              </div>
            </div>
            <button
              type="button"
              className={styles.closeBtn}
              aria-label="Cerrar"
              onClick={() => onClose?.()}
            >
              ✕
            </button>
          </header>

          <div className={styles.bodyRow}>
            <aside className={styles.sidebar}>
              {GROUPS.map((g, idx) => (
                <div key={g.key}>
                  <button
                    type="button"
                    className={`${styles.sidebarBtn} ${activeGroup === g.key ? styles.sidebarBtnActive : ''}`}
                    onClick={() => selectGroup(g.key)}
                  >
                    <span className={styles.sidebarDot} />
                    {g.label}
                  </button>
                  {idx === 0 ? <div className={styles.divider} /> : null}
                </div>
              ))}
            </aside>

            <div className={styles.main}>
              <div className={styles.legend}>
                <div className={styles.legendItem}>
                  <span className={`${styles.dot} ${styles.dotGreen}`} />
                  Disponible
                </div>
                <span className={styles.legendSep}>·</span>
                <div className={styles.legendItem}>
                  <span className={`${styles.dot} ${styles.dotRed}`} />
                  Lleno
                </div>
                <span className={styles.legendSep}>·</span>
                <div className={styles.legendItem}>
                  <span className={`${styles.dot} ${styles.dotYellow}`} />
                  Saturado
                </div>
                <span className={styles.legendSep}>·</span>
                <div className={styles.legendItem}>
                  <span className={`${styles.dot} ${styles.dotGray}`} />
                  Mantenimiento
                </div>
              </div>

              <div className={styles.gridScroll}>
                <div
                  key={displayedGroup}
                  className={`${styles.grid} ${gridClass}`}
                >
                  {gridServers.map((server) => {
                    const isMaint = server.status === 'maintenance';
                    const isSel = selectedId === server.id;
                    const dotCls =
                      server.status === 'available'
                        ? styles.dotGreen
                        : server.status === 'full'
                          ? styles.dotRed
                          : server.status === 'saturated'
                            ? styles.dotYellow
                            : styles.dotGray;
                    const lblCls =
                      server.status === 'available'
                        ? styles.labelAvail
                        : server.status === 'full'
                          ? styles.labelFull
                          : server.status === 'saturated'
                            ? styles.labelSat
                            : styles.labelMaint;

                    return (
                      <button
                        key={server.id}
                        type="button"
                        className={[
                          styles.card,
                          isSel ? styles.cardSelected : '',
                          isMaint ? styles.cardDisabled : '',
                        ]
                          .filter(Boolean)
                          .join(' ')}
                        onClick={() => onCardClick(server)}
                        disabled={isMaint}
                      >
                        {server.isNew ? (
                          <span className={styles.badge}>NUEVO</span>
                        ) : null}
                        <span className={`${styles.dot} ${dotCls}`} />
                        <div className={styles.cardBody}>
                          <div className={styles.cardName}>{server.name}</div>
                        </div>
                        <span
                          className={`${styles.statusLabel} ${lblCls}`}
                        >
                          {STATUS_LABEL[server.status]}
                        </span>
                      </button>
                    );
                  })}
                </div>
              </div>
            </div>
          </div>

          <footer className={styles.footer}>
            <div className={styles.footerText}>
              Servidor seleccionado:{' '}
              {selected ? (
                <span className={styles.footerName}>{selected.name}</span>
              ) : (
                <span className={styles.footerName}>—</span>
              )}
            </div>
            <button
              type="button"
              className={styles.connectBtn}
              disabled={!selected}
              onClick={() => selected && onConnect(selected.name)}
            >
              CONECTAR ▶
            </button>
          </footer>
        </div>
      </div>
    </div>
  );
}
