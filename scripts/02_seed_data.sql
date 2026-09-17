-- =============================================================================
-- MIRA - Datos Semilla Iniciales (Entrega 1)
-- =============================================================================

-- 1. area_conocimiento
INSERT INTO area_conocimiento (gran_area, area, disciplina, activo) VALUES
('Ingeniería y Tecnología', 'Ingeniería de Sistemas', 'Ingeniería de Software', TRUE),
('Ciencias Naturales', 'Ciencias de la Computación', 'Inteligencia Artificial', TRUE),
('Ciencias Sociales', 'Economía y Negocios', 'Gestión de Proyectos', TRUE)
ON CONFLICT DO NOTHING;

-- 2. objetivo_desarrollo_sostenible
INSERT INTO objetivo_desarrollo_sostenible (nombre, categoria, activo) VALUES
('Educación de Calidad', 'Social', TRUE),
('Industria, Innovación e Infraestructura', 'Económico', TRUE),
('Acción por el Clima', 'Ambiental', TRUE)
ON CONFLICT DO NOTHING;

-- 3. area_aplicacion
INSERT INTO area_aplicacion (nombre, activo) VALUES
('Salud y Telemedicina', TRUE),
('Educación Virtual', TRUE),
('Sector Financiero y Fintech', TRUE)
ON CONFLICT DO NOTHING;

-- 4. termino_clave (PK string)
INSERT INTO termino_clave (termino, termino_ingles, activo) VALUES
('Inteligencia Artificial', 'Artificial Intelligence', TRUE),
('Computación en la Nube', 'Cloud Computing', TRUE),
('Arquitectura de Software', 'Software Architecture', TRUE)
ON CONFLICT (termino) DO NOTHING;

-- 5. universidad
INSERT INTO universidad (nombre, tipo, ciudad, activo) VALUES
('Universidad de San Buenaventura Medellín', 'Privada', 'Medellín', TRUE),
('Universidad de Antioquia', 'Pública', 'Medellín', TRUE),
('Universidad Nacional de Colombia', 'Pública', 'Bogotá', TRUE)
ON CONFLICT DO NOTHING;

-- 6. linea_investigacion
INSERT INTO linea_investigacion (nombre, descripcion, activo) VALUES
('Ingeniería de Software y Sistemas Distribuidos', 'Investigación orientada a arquitecturas modernas, microservicios y calidad de software.', TRUE),
('Inteligencia Artificial y Analítica de Datos', 'Enfoque en modelos de aprendizaje automático y visión por computadora.', TRUE),
('Ciberseguridad y Redes', 'Investigación en seguridad informática, protocolos criptográficos y protección de infraestructuras.', TRUE)
ON CONFLICT DO NOTHING;

