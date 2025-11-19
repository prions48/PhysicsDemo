using System.Security.Cryptography.X509Certificates;

namespace PhysicsDemo.Data.GameData
{
    public class PhysicsService
    {
        private readonly PhysicsContext _context;
        public PhysicsService(PhysicsContext context)
        {
            _context = context;
        }
        private string GenerateCode()
        {
            string code = CreateCode();
            while (_context.PhysicsGames.Any(e => e.GameEnd == null && e.JoinCode == code))
                code = CreateCode();
            return code;
        }
        private string CreateCode()
        {
            Random r = new();
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            string ret = "";
            for (int i = 0; i < 6; i++)
            {
                ret += chars[r.Next(chars.Length)];
            }
            return ret;
        }
        #region Game functions
        #region Game loaders
        #region Util functions for loading
        private PhysicsGame? EnrichGame(PhysicsGame? game)
        {
            if (game == null)
                return null;
            Guid gameid = game.ID;
            game.Players = _context.PhysicsPlayers.Where(e => e.GameID == gameid).ToList();
            List<PhysicsTurn> turns = _context.PhysicsTurns.Where(e => gameid == e.GameID).ToList();
            foreach (PhysicsPlayer player in game.Players)
            {
                player.Turns = turns.Where(e => e.PlayerID == player.ID).OrderBy(e => e.RoundNumber).ToList();
            }
            return RecalcGame(game);
        }
        private List<PhysicsGame> EnrichGames(List<PhysicsGame> games)
        {
            List<PhysicsPlayer> players = _context.PhysicsPlayers.Where(e => games.Select(f => f.ID).Contains(e.GameID)).ToList();
            foreach (PhysicsGame game in games)
            {
                Guid gameid = game.ID;
                List<PhysicsTurn> turns = _context.PhysicsTurns.Where(e => e.GameID == gameid).ToList();
                game.Players = players.Where(e => e.GameID == gameid).ToList();
                foreach (PhysicsPlayer player in game.Players)
                {
                    player.Turns = turns.Where(e => e.PlayerID == player.ID).OrderBy(e => e.RoundNumber).ToList();
                }
            }
            return RecalcTurn(games);
        }
        private List<PhysicsGame> RecalcTurn(List<PhysicsGame> games)
        {
            for (int i = 0; i < games.Count; i++)
            {
                games[i] = RecalcGame(games[i])!;
                _context.PhysicsGames.Update(games[i]);
            }
            _context.SaveChanges();
            return games;
        }
        private PhysicsGame? RecalcGame(PhysicsGame? game)
        {
            if (game == null)
                return game;
            if (game.Turns.Count == 0)
            {
                game.CurrentRound = 1;
                return game;
            }
            List<int> rounds = game.Turns.Select(e => e.RoundNumber).ToList();
            int maxrd = rounds.Max();
            if (rounds.Count(e => e == maxrd) == game.NumPlayers)
            {
                game.CurrentRound = maxrd + 1;
            }
            else
            {
                game.CurrentRound = maxrd;
            }
            return game;
        }
        #endregion
        public List<PhysicsGame> GetGamesByCreator(Guid creatorid)
        {
            return EnrichGames(_context.PhysicsGames.Where(e => e.CreatorID == creatorid).ToList());
        }
        public List<PhysicsGame> GetGamesByUser(Guid userid)
        {
            return EnrichGames(
                _context.PhysicsGames
                .Join(
                    _context.PhysicsPlayers.Where(e => e.PlayerID == userid),
                    pg => pg.ID,
                    pp => pp.GameID,
                    ( pg, pp) => pg
                )
                //.Where(e => e.CreatorID == userid || e.GameEnd == null)//include history or nah?
                .Distinct()
                .ToList()
            );
        }
        public List<PhysicsGame> GetOpenGames()
        {
            return EnrichGames(_context.PhysicsGames.Where(e => e.GameEnd == null).ToList());
        }
        public PhysicsGame? GetGameByCode(string code)
        {
            return EnrichGame(_context.PhysicsGames.FirstOrDefault(e => e.JoinCode == code && e.GameEnd == null));
        }
        public PhysicsGame? GetGameByID(Guid id)
        {
            return EnrichGame(_context.PhysicsGames.FirstOrDefault(e => e.ID == id));
        }
        public List<PhysicsPlayer> ReloadPlayers(Guid gameid)
        {
            List<PhysicsPlayer> players = _context.PhysicsPlayers.Where(e => e.GameID == gameid).ToList();
            List<PhysicsTurn> turns = _context.PhysicsTurns.Where(e => gameid == e.GameID).ToList();
            foreach (PhysicsPlayer player in players)
            {
                player.Turns = turns.Where(e => e.PlayerID == player.ID).ToList();
            }
            return players;
        }
        #endregion

        #region Game creators/editors
        public void CreateGame(PhysicsGame game)
        {
            game.JoinCode = GenerateCode();
            _context.PhysicsGames.Add(game);
            _context.PhysicsPlayers.Add(game.Players.First());
            _context.SaveChanges();
        }
        public void CreatePlayer(PhysicsPlayer player)
        {
            _context.PhysicsPlayers.Add(player);
            _context.SaveChanges();
        }
        public void RemovePlayer(PhysicsPlayer player)
        {
            _context.PhysicsPlayers.Remove(player);
            _context.SaveChanges();
        }
        public void CreateTurn(PhysicsTurn turn)
        {
            _context.PhysicsTurns.Add(turn);
            _context.SaveChanges();
        }
        #endregion
        #endregion
    }
}